using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Exceptions;
using IdelPog.SimulationEngine.Currency;
using IdelPog.SimulationEngine.Currency.Commands;
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
            new CurrencyBootstrapper().Initialize(BufferMessenger, BufferManager);

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

                Assert.That(_currencyUpdateResponseListener.Buffer, Is.Not.Null);
                Assert.That(_currencyUpdateResponseListener.Buffer!, Has.Count.EqualTo(1));
            });
        }

        private void AssertErrorListener(bool wasCalled)
        {
            Assert.That(_currencyUpdateErrorListener.WasCalled, Is.EqualTo(wasCalled));
        }

        private void AssertUpdateResponse(CurrencyUpdateResponse response, CurrencyUpdate expected)
        {
            Assert.Multiple(() =>
            {
                Assert.That(response.Amount, Is.EqualTo(expected.Amount));
                Assert.That(response.CurrencyType, Is.EqualTo(expected.CurrencyType));
                Assert.That(response.Action, Is.EqualTo(expected.Action));
            });
        }

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        public void Positive_SendAddGoldUpdate_ProducesSingleCurrencyUpdate(int amount)
        {
            CurrencyUpdate[] currencyUpdates = Enumerable.Repeat(_addGoldCommand, amount).ToArray();

            SendGoldCreationBuffer();
            SendCurrencyTradeBuffer(currencyUpdates);
            AssertErrorListener(false);
            AssertUpdateListener(true);

            CurrencyUpdateResponse[] currencyUpdate = _currencyUpdateResponseListener.Buffer!.ToArray();
            AssertUpdateResponse(currencyUpdate[0],
                new CurrencyUpdate { Action = ActionType.ADD, CurrencyType = CurrencyType.GOLD, Amount = _addGoldCommand.Amount * (uint) amount });
        }

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        public void Positive_SendRemoveGoldUpdate_ProducesSingleCurrencyUpdate(int amount)
        {
            CurrencyUpdate[] currencyUpdates = Enumerable.Repeat(_removeGoldCommand, amount).ToArray();

            SendGoldCreationBuffer(_removeGoldCommand.Amount * (uint) amount);
            SendCurrencyTradeBuffer(currencyUpdates);
            AssertErrorListener(false);
            AssertUpdateListener(true);

            CurrencyUpdateResponse[] currencyUpdate = _currencyUpdateResponseListener.Buffer!.ToArray();
            AssertUpdateResponse(currencyUpdate[0],
                new CurrencyUpdate { Action = ActionType.REMOVE, Amount = _removeGoldCommand.Amount * (uint) amount, CurrencyType = CurrencyType.GOLD });
        }

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        public void Positive_SendMixedUpdates_ProducesSingleCorrectUpdate(int amount)
        {
            List<CurrencyUpdate> currencyUpdates = [];
            currencyUpdates.AddRange(Enumerable.Repeat(_removeGoldCommand, amount));
            currencyUpdates.AddRange(Enumerable.Repeat(_addGoldCommand, amount));

            SendGoldCreationBuffer(_removeGoldCommand.Amount * (uint) amount);
            SendCurrencyTradeBuffer(currencyUpdates.ToArray());
            AssertErrorListener(false);
            AssertUpdateListener(true);

            CurrencyUpdateResponse[] currencyUpdate = _currencyUpdateResponseListener.Buffer!.ToArray();
            uint finalAmount = _addGoldCommand.Amount * (uint) amount - _removeGoldCommand.Amount * (uint) amount;
            AssertUpdateResponse(currencyUpdate[0], new CurrencyUpdate { Action = ActionType.ADD, Amount = finalAmount, CurrencyType = CurrencyType.GOLD });
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