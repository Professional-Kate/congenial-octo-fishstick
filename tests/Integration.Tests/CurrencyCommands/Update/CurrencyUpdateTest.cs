using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Common.Errors;
using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Exceptions;
using IdelPog.SimulationEngine.Currency.Exceptions;
using IdelPog.SimulationEngine.Currency.Responses;
using IdelPog.Validation.Exceptions;

namespace Integration.Tests.CurrencyCommands.Update
{
    [TestFixture]
    public class CurrencyFlowTest : ManagedBuffer
    {
        private CurrencyUpdateResponseListener _currencyUpdateResponseListener;
        private CurrencyUpdateErrorListener _currencyUpdateErrorListener;

        private CurrencyUpdate _addGoldCommand;
        private CurrencyUpdate _removeGoldCommand;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _addGoldCommand = new CurrencyUpdate
            {
                Action = ActionType.ADD,
                Amount = 10,
                CurrencyType = CurrencyType.GOLD
            };

            _removeGoldCommand = new CurrencyUpdate
            {
                Action = ActionType.REMOVE,
                Amount = 1,
                CurrencyType = CurrencyType.GOLD
            };
        }

        [SetUp]
        public void SetUp()
        {
            _currencyUpdateResponseListener = new CurrencyUpdateResponseListener();
            _currencyUpdateErrorListener = new CurrencyUpdateErrorListener();

            ManagedSubscribe(_currencyUpdateResponseListener);
            ManagedSubscribe(_currencyUpdateErrorListener);
        }

        private void SendGoldCreationBuffer(uint startingAmount = 0)
        {
            IBuffer<CurrencyCreation> buffer = BufferManager.RequestBuffer<CurrencyCreation>(new BufferRequest(1));
            buffer.Assign([new CurrencyCreation { CurrencyType = CurrencyType.GOLD, StartingAmount = startingAmount }]);
            buffer.MarkReady();
        }

        private void SendCurrencyTradeBuffer(CurrencyUpdate[] trades)
        {
            IBuffer<CurrencyUpdate> buffer = BufferManager.RequestBuffer<CurrencyUpdate>(new BufferRequest(trades.Length));
            buffer.Assign(trades);
            buffer.MarkReady();
        }

        private void AssertUpdateListener(bool wasCalled)
        {
            Assert.Multiple(() =>
            {
                Assert.That(_currencyUpdateResponseListener.WasCalled, Is.EqualTo(wasCalled));

                if (wasCalled == false)
                {
                    return;
                }

                Assert.That(_currencyUpdateResponseListener.Item.CurrencyUpdates, Is.Not.Null);
                Assert.That(_currencyUpdateResponseListener.Item.CurrencyUpdates, Has.Length.EqualTo(1));
            });
        }

        private void AssertErrorListener(bool wasCalled)
        {
            Assert.That(_currencyUpdateErrorListener.WasCalled, Is.EqualTo(wasCalled));
        }

        private void AssertUpdateResponse(CurrencyUpdateResponse response, CurrencyUpdate[] expected)
        { 
            Assert.That(response.CurrencyUpdates, Is.EquivalentTo(expected));
        }

        [TestCase(1u)]
        [TestCase(10u)]
        [TestCase(100u)]
        public void Positive_SendAddGoldUpdate_ProducesSingleCurrencyUpdate(uint amount)
        {
            CurrencyUpdate[] currencyUpdates = Enumerable.Repeat(_addGoldCommand, (int) amount).ToArray();

            SendGoldCreationBuffer();
            SendCurrencyTradeBuffer(currencyUpdates);
            AssertErrorListener(false);
            AssertUpdateListener(true);

            AssertUpdateResponse(_currencyUpdateResponseListener.Item, [_addGoldCommand with { Amount = amount * 10 }]);
        }

        [TestCase(1u)]
        [TestCase(10u)]
        [TestCase(100u)]
        public void Positive_SendRemoveGoldUpdate_ProducesSingleCurrencyUpdate(uint amount)
        {
            CurrencyUpdate[] currencyUpdates = Enumerable.Repeat(_removeGoldCommand, (int) amount).ToArray();

            SendGoldCreationBuffer(_removeGoldCommand.Amount * amount);
            SendCurrencyTradeBuffer(currencyUpdates);
            AssertErrorListener(false);
            AssertUpdateListener(true);

            AssertUpdateResponse(_currencyUpdateResponseListener.Item, [_removeGoldCommand with { Amount = amount }]);
        }

        [TestCase(1u)]
        [TestCase(10u)]
        [TestCase(100u)]
        public void Positive_SendMixedUpdates_ProducesSingleCorrectUpdate(uint amount)
        {
            List<CurrencyUpdate> currencyUpdates = [];
            currencyUpdates.AddRange(Enumerable.Repeat(_removeGoldCommand, (int) amount));
            currencyUpdates.AddRange(Enumerable.Repeat(_addGoldCommand, (int) amount));

            SendGoldCreationBuffer(_removeGoldCommand.Amount * amount);
            SendCurrencyTradeBuffer(currencyUpdates.ToArray());
            AssertErrorListener(false);
            AssertUpdateListener(true);

            uint finalAmount = _addGoldCommand.Amount * amount - _removeGoldCommand.Amount * amount;
            AssertUpdateResponse(_currencyUpdateResponseListener.Item, [_addGoldCommand with { Amount = finalAmount }]);
        }

        [Test]
        public void Negative_OneCommand_NotFoundCurrency_NoUpdate_SendsErrorDTO()
        {
            Assert.DoesNotThrow(() => SendCurrencyTradeBuffer([_addGoldCommand]));
            AssertErrorListener(true);
            AssertUpdateListener(false);

            CurrencyUpdateError error = _currencyUpdateErrorListener.CurrencyUpdateError;
            
            ControllerThrownException controllerException = (ControllerThrownException) error.BaseErrorDetails.Exception;
            Assert.Multiple(() =>
            {
                Assert.That(error.BaseErrorDetails.Exception, Is.TypeOf<ControllerThrownException>());
                Assert.That(controllerException.InnerException, Is.TypeOf<NotFoundException<CurrencyType>>());
            });
        }
        
        [Test]
        public void Negative_OneCommand_NotEnoughCurrency_NoUpdate_SendsErrorDTO()
        {
            const int goldAmount = 10;
            SendGoldCreationBuffer(goldAmount);

            CurrencyUpdate notEnoughGoldUpdate = new()
            {
                Action = ActionType.REMOVE,
                Amount = 20,
                CurrencyType = CurrencyType.GOLD
            };

            Assert.DoesNotThrow(() => SendCurrencyTradeBuffer([notEnoughGoldUpdate]));
            AssertErrorListener(true);
            AssertUpdateListener(false);

            CurrencyUpdateError error = _currencyUpdateErrorListener.CurrencyUpdateError;

            Assert.Multiple(() =>
            {
                if (error.BaseErrorDetails.Exception is NotEnoughCurrencyException exception)
                {
                    Assert.That(exception, Is.TypeOf<NotEnoughCurrencyException>());
                    Assert.That(exception.CurrencyTypeContext, Is.EqualTo(notEnoughGoldUpdate.CurrencyType));
                    Assert.That(exception.RemoveAmount, Is.EqualTo(notEnoughGoldUpdate.Amount));
                    Assert.That(exception.CurrencyAmount, Is.EqualTo(goldAmount));
                }
            });
        }

        [Test]
        public void Negative_MultipleCommands_SomeValidOneError_NoUpdate_SendsErrorDTO()
        {
            SendGoldCreationBuffer();

            CurrencyUpdate notFoundUpdate = new()
            {
                Action = ActionType.ADD,
                Amount = 10,
                CurrencyType = CurrencyType.GEMS
            };

            Assert.DoesNotThrow(() => SendCurrencyTradeBuffer([_addGoldCommand, _addGoldCommand, notFoundUpdate]));
            AssertErrorListener(true);
            AssertUpdateListener(false);

            CurrencyUpdateError error = _currencyUpdateErrorListener.CurrencyUpdateError;
            ControllerThrownException controllerException = (ControllerThrownException) error.BaseErrorDetails.Exception;
            Assert.Multiple(() =>
            {
                Assert.That(_currencyUpdateResponseListener.WasCalled, Is.False);
                Assert.That(_currencyUpdateErrorListener.WasCalled, Is.True);
                Assert.That(_currencyUpdateErrorListener.CurrencyUpdateError.BaseErrorDetails.Exception, Is.TypeOf<ControllerThrownException>());
                Assert.That(controllerException.InnerException, Is.TypeOf<NotFoundException<CurrencyType>>());
            });
        }
    }
}