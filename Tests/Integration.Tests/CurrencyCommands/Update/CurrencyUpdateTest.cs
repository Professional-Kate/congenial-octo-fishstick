using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Currency.Exceptions;

namespace IdelPog.Integration.Tests.CurrencyCommands.Update
{
    [TestFixture]
    public sealed class CurrencyFlowTest : ManagedTestBuffer
    {
        private CurrencyUpdateResponseListener _currencyUpdateResponseListener;
        private CurrencyUpdateErrorListener _currencyUpdateErrorListener;

        private CurrencyUpdate _addGoldCommand;
        private CurrencyCreation _goldCreation;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _addGoldCommand = new CurrencyUpdate
            {
                ActionType = ActionType.ADD,
                Amount = 10,
                CurrencyType = CurrencyType.GOLD
            };

            _goldCreation = new CurrencyCreation
            {
                StartingAmount = 0,
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

        private void SendCreations(params CurrencyCreation[] currencyCreation)
        {
            IBuffer<CurrencyCreation> buffer = BufferManager.RequestBuffer<CurrencyCreation>(new BufferRequest(currencyCreation.Length));
            buffer.Assign(currencyCreation);
            buffer.MarkReady();
        }

        private void SendUpdates(params CurrencyUpdate[] trades)
        {
            IBuffer<CurrencyUpdate> buffer = BufferManager.RequestBuffer<CurrencyUpdate>(new BufferRequest(trades.Length));
            buffer.Assign(trades);
            buffer.MarkReady();
        }

        private void AssertUpdateListener(bool wasCalled)
        { 
            Assert.That(_currencyUpdateResponseListener.WasCalled, Is.EqualTo(wasCalled));
        }
        
        private void AssertResponseLength(int length)
        {
            Assert.That(_currencyUpdateResponseListener.CurrencyUpdateResponses, Has.Length.EqualTo(length));
        }

        private static void AssertUpdateResponse(CurrencyUpdateResponse responses, CurrencyUpdate expected)
        {
            Assert.Multiple(() =>
            {
                Assert.That(responses.CurrencyAmount, Is.EqualTo(expected.Amount));
                Assert.That(responses.CurrencyType, Is.EqualTo(expected.CurrencyType));
            });
        }

        private void AssertErrorListener(bool wasCalled)
        {
            Assert.That(_currencyUpdateErrorListener.WasCalled, Is.EqualTo(wasCalled));
        }
        
        private void AssertErrorLength(int length)
        {
            Assert.That(_currencyUpdateErrorListener.CurrencyUpdateError.CurrencyUpdates, Has.Length.EqualTo(length));
        }

        private static void AssertError(Type exception, CurrencyUpdate[] updates, CurrencyUpdateError error)
        {
            Assert.Multiple(() =>
            {
                Assert.That(error.BaseErrorDetails.Exception, Is.TypeOf<ControllerThrownException>());
                Assert.That(error.BaseErrorDetails.Exception.InnerException, Is.TypeOf(exception));
                Assert.That(error.CurrencyUpdates, Is.EqualTo(updates));
            });
        }

        [Test]
        public void Positive_SendAddUpdate_ShouldReturnNewCurrencyAmount()
        {
            SendCreations(_goldCreation with { StartingAmount = 50 });
            
            SendUpdates(_addGoldCommand);
            
            AssertErrorListener(false);
            AssertUpdateListener(true);
            AssertResponseLength(1);
            AssertUpdateResponse(_currencyUpdateResponseListener.CurrencyUpdateResponses[0], _addGoldCommand with { Amount = 60 });
        }

        [Test]
        public void Positive_SendAddGoldUpdate_ProducesSingleCurrencyUpdate()
        {
            SendCreations(_goldCreation);
            
            SendUpdates(_addGoldCommand);
            
            AssertErrorListener(false);
            AssertUpdateListener(true);
            AssertResponseLength(1);
            AssertUpdateResponse(_currencyUpdateResponseListener.CurrencyUpdateResponses[0], _addGoldCommand);
        }

        [Test]
        public void Positive_SendRemoveGoldUpdate_ProducesSingleCurrencyUpdate()
        {
            SendCreations(_goldCreation with {  StartingAmount = _addGoldCommand.Amount });
            
            SendUpdates(_addGoldCommand with { ActionType = ActionType.REMOVE });
            
            AssertErrorListener(false);
            AssertUpdateListener(true);
            AssertResponseLength(1);
            
            AssertUpdateResponse(_currencyUpdateResponseListener.CurrencyUpdateResponses[0], _addGoldCommand with { ActionType = ActionType.REMOVE, Amount = 0 });
        }

        [Test]
        public void Positive_SendMixedUpdates_ProducesSingleCorrectUpdate()
        {
            SendCreations(_goldCreation);
            CurrencyUpdate removeGold = _addGoldCommand with { ActionType = ActionType.REMOVE };
            
            SendUpdates(removeGold, removeGold, _addGoldCommand, _addGoldCommand, _addGoldCommand);
            
            AssertErrorListener(false);
            AssertUpdateListener(true);
            AssertResponseLength(1);
            
            AssertUpdateResponse(_currencyUpdateResponseListener.CurrencyUpdateResponses[0], _addGoldCommand);
        }

        [Test]
        public void Positive_SendMultipleCurrencyTypes_DispatchesMultipleResponses()
        {
            SendCreations(_goldCreation with { CurrencyType = CurrencyType.GEMS }, _goldCreation);
            
            SendUpdates(_addGoldCommand, _addGoldCommand with { CurrencyType = CurrencyType.GEMS });
            
            AssertErrorListener(false);
            AssertUpdateListener(true);
            AssertResponseLength(2);
            
            AssertUpdateResponse(_currencyUpdateResponseListener.CurrencyUpdateResponses[0], _addGoldCommand);
            AssertUpdateResponse(_currencyUpdateResponseListener.CurrencyUpdateResponses[1], _addGoldCommand with { CurrencyType = CurrencyType.GEMS });
        }

        [Test]
        public void Negative_OneCommand_NotFoundCurrency_NoUpdate_SendsError()
        {
            Assert.DoesNotThrow(() => SendUpdates(_addGoldCommand));
            
            AssertErrorListener(true);
            AssertUpdateListener(false);
            AssertErrorLength(1);
            AssertError(typeof(NotFoundException<CurrencyType>), [_addGoldCommand], _currencyUpdateErrorListener.CurrencyUpdateError);
        }
        
        [Test]
        public void Negative_OneCommand_NotEnoughCurrency_NoUpdate_SendsError()
        {
            SendCreations(_goldCreation);
            CurrencyUpdate removeGold = _addGoldCommand with { ActionType = ActionType.REMOVE };

            Assert.DoesNotThrow(() => SendUpdates(removeGold));
            
            AssertErrorListener(true);
            AssertUpdateListener(false);
            AssertErrorLength(1);
            AssertError(typeof(NotEnoughCurrencyException), [removeGold], _currencyUpdateErrorListener.CurrencyUpdateError);
        }

        [Test]
        public void Negative_MultipleCommands_SomeValidOneError_NoUpdate_SendsError()
        {
            SendCreations(_goldCreation);
            CurrencyUpdate addGems = _addGoldCommand with { CurrencyType = CurrencyType.GEMS };

            Assert.DoesNotThrow(() => SendUpdates(_addGoldCommand, addGems));
            
            AssertErrorListener(true);
            AssertUpdateListener(false);
            AssertErrorLength(2);
            AssertError(typeof(NotFoundException<CurrencyType>), [_addGoldCommand, addGems], _currencyUpdateErrorListener.CurrencyUpdateError);
        }
    }
}