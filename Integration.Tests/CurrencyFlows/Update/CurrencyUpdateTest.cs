using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Assertions.Pipelines;
using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Factory;
using IdelPog.Messaging.Orchestration;
using IdelPog.SimulationEngine.Currency;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.DTO;
using IdelPog.SimulationEngine.Structures;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Interfaces;
using IdelPog.Validation.Constants;
using IdelPog.Validation.Exceptions;

namespace Integration.Tests.CurrencyFlows.Update
{
    [TestFixture]
    public class CurrencyFlowTest
    {
        private IBufferManager _bufferManager;
        private IBufferMessenger _bufferMessenger;
        private IBufferDispatcher _bufferDispatcher;
        private CurrencyUpdateListener _currencyUpdateListener;
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
            IAssertNotNull assertNotNull = new AssertNotNull(new ThrowHandler());
            IAssertListenerFound assertListenerFound = new AssertListenerFound(new ThrowHandler());
            IAssertCollectionSize assertCollectionSize = new AssertCollectionSize(new ThrowHandler());
            IAssertValidCollectionSize assertValidCollectionSize = new AssertValidCollectionSize(new ThrowHandler());
            IAssertBufferState assertBufferState = new AssertBufferState(new ThrowHandler());

            IBufferAsserter bufferAsserter = new BufferAsserter(assertNotNull, assertCollectionSize, assertValidCollectionSize);
            IBufferFactory bufferFactory = new BufferFactory(bufferAsserter, assertBufferState, assertNotNull);
            
            BufferMessenger bufferMessenger = new(assertNotNull, assertListenerFound);
            _bufferMessenger = bufferMessenger;
            _bufferDispatcher = bufferMessenger;
            _bufferManager = new BufferManager(bufferFactory, _bufferDispatcher, assertNotNull);
            
            new CurrencyBootstrapper().Initialize(_bufferMessenger, _bufferManager);
            _currencyUpdateListener = new CurrencyUpdateListener();
            _bufferMessenger.Subscribe(_currencyUpdateListener);
            
            _currencyUpdateErrorListener = new CurrencyUpdateErrorListener();
            _bufferMessenger.Subscribe(_currencyUpdateErrorListener);
        }

        private void SendGoldCreationBuffer(int startingAmount = 0)
        {
            IBuffer<CurrencyCreation> buffer = _bufferManager.RequestBuffer<CurrencyCreation>(new BufferRequest(1));
            buffer.Assign([new CurrencyCreation { CurrencyType = CurrencyType.GOLD, StartingAmount = startingAmount}]);
            buffer.MarkReady();
        }

        private void SendCurrencyTradeBuffer(CurrencyUpdate[] trades)
        {
            IBuffer<CurrencyUpdate> buffer = _bufferManager.RequestBuffer<CurrencyUpdate>(new BufferRequest(trades.Length));
            buffer.Assign(trades);
            buffer.MarkReady();
        }

        private void AssertUpdateListener(bool wasCalled)
        {
            Assert.Multiple(() =>
            {
                Assert.That(_currencyUpdateListener.WasCalled, Is.EqualTo(wasCalled));
                
                if (wasCalled == false)
                {
                    return;
                }
                
                Assert.That(_currencyUpdateListener.Buffer, Is.Not.Null);
                Assert.That(_currencyUpdateListener.Buffer!, Has.Count.EqualTo(1));
            });
        }
        
        private void AssertErrorListener(int bufferLength, bool wasCalled)
        {
            Assert.Multiple(() =>
            {
                Assert.That(_currencyUpdateErrorListener.WasCalled, Is.EqualTo(wasCalled));
                
                if (wasCalled == false)
                {
                    return;
                }
                
                Assert.That(_currencyUpdateErrorListener.CurrencyUpdateErrorDTO.CurrencyUpdates, Is.Not.Null);
                Assert.That(_currencyUpdateErrorListener.CurrencyUpdateErrorDTO.CurrencyUpdates, Has.Length.EqualTo(bufferLength));
            });
        }

        private void AssertUpdateResponse(CurrencyUpdateDTO dto, CurrencyUpdate expected)
        {
            Assert.Multiple(() =>
            {
                Assert.That(dto.Amount, Is.EqualTo(expected.Amount));
                Assert.That(dto.Currency, Is.EqualTo(expected.CurrencyType));
                Assert.That(dto.Action, Is.EqualTo(expected.Action));
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
            AssertErrorListener(0, false);
            AssertUpdateListener(true);
            
            CurrencyUpdateDTO[] currencyUpdate = _currencyUpdateListener.Buffer!.ToArray();
            AssertUpdateResponse(currencyUpdate[0], new CurrencyUpdate { Action = ActionType.ADD, CurrencyType = CurrencyType.GOLD, Amount = _addGoldCommand.Amount * amount });
        }

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        public void Positive_SendRemoveGoldUpdate_ProducesSingleCurrencyUpdate(int amount)
        {
            CurrencyUpdate[] currencyUpdates = Enumerable.Repeat(_removeGoldCommand, amount).ToArray();
            
            SendGoldCreationBuffer(_removeGoldCommand.Amount * amount);
            SendCurrencyTradeBuffer(currencyUpdates);
            AssertErrorListener(0, false);
            AssertUpdateListener(true);
            
            CurrencyUpdateDTO[] currencyUpdate = _currencyUpdateListener.Buffer!.ToArray();
            AssertUpdateResponse(currencyUpdate[0], new CurrencyUpdate { Action = ActionType.REMOVE, Amount = _removeGoldCommand.Amount * amount, CurrencyType = CurrencyType.GOLD });
        }

        [Test]
        public void Negative_OneCommand_NotFoundCurrency_NoUpdate_SendsErrorDTO()
        {
            Assert.DoesNotThrow(() => SendCurrencyTradeBuffer([_addGoldCommand]));
            AssertErrorListener(1, true);
            AssertUpdateListener(false);
            
            CurrencyUpdateErrorDTO errorDTO = _currencyUpdateErrorListener.CurrencyUpdateErrorDTO;
            AssertUpdateResponse(errorDTO.CurrencyUpdates[0], _addGoldCommand);
            
            Assert.Multiple(() =>
            {
                Assert.That(errorDTO.ErrorDetails.ErrorMessage, Is.EqualTo(string.Format(ExceptionConstants.NOT_FOUND_MESSAGE, _addGoldCommand.CurrencyType)));
                Assert.That(errorDTO.ErrorDetails.Exception, Is.TypeOf<NotFoundException>());
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
            AssertErrorListener(3, true);
            AssertUpdateListener(false);

            Assert.Multiple(() =>
            {
                Assert.That(_currencyUpdateListener.WasCalled, Is.False);
                Assert.That(_currencyUpdateErrorListener.WasCalled, Is.True);
                
                Assert.That(_currencyUpdateErrorListener.CurrencyUpdateErrorDTO.ErrorDetails.ErrorMessage, Is.EqualTo(string.Format(ExceptionConstants.NOT_FOUND_MESSAGE, notFoundUpdate.CurrencyType)));
                Assert.That(_currencyUpdateErrorListener.CurrencyUpdateErrorDTO.ErrorDetails.Exception, Is.TypeOf<NotFoundException>());
            });
        }
    }
}