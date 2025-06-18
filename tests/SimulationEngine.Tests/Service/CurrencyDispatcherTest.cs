using IdelPog.Messaging.Collection;
using IdelPog.Messaging.Orchestration;
using IdelPog.SimulationEngine.Flows.Currency;
using IdelPog.SimulationEngine.Flows.Currency.Assertions;
using IdelPog.SimulationEngine.Flows.Currency.Exceptions;
using IdelPog.SimulationEngine.Structures.Enums;
using IdelPog.Validation.Exceptions;
using IdelPogTests.Utils;
using Moq;

namespace IdelPogTests.Service
{
    [TestFixture]
    public class CurrencyDispatcherTest
    {
        private ICurrencyDispatcher _currencyDispatcher { get; set; }
        private Mock<IBufferManager> _bufferManagerMock { get; set; }
        private Mock<ICurrencyUpdateFactory>  _currencyUpdateFactoryMock { get; set; }
        private Mock<ICurrencyDispatcherAsserter> _asserterMock { get; set; }
        
        private Mock<IBuffer<CurrencyUpdateDTO>> _bufferMock { get; set; }
        private IReadOnlyList<CurrencyTrade> _trades { get; set; }
        private IReadOnlyList<CurrencyUpdateDTO> _updateDTOs { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _bufferMock = new Mock<IBuffer<CurrencyUpdateDTO>>();
            _bufferManagerMock = new Mock<IBufferManager>();
            _currencyUpdateFactoryMock = new Mock<ICurrencyUpdateFactory>();
            _asserterMock = new Mock<ICurrencyDispatcherAsserter>();
            
            _currencyDispatcher = new CurrencyDispatcher(_bufferManagerMock.Object,  _currencyUpdateFactoryMock.Object, _asserterMock.Object);

            CreateTestObjects();
        }

        [TearDown]
        public void TearDown()
        {
            _bufferManagerMock.Reset();
            _currencyUpdateFactoryMock.Reset();
            _asserterMock.Reset();
        }

        private void CreateTestObjects()
        {
            _trades =
                [
                
                    TestUtils.CreateTrade(10, CurrencyType.FOOD, ActionType.ADD),
                    TestUtils.CreateTrade(10, CurrencyType.WOOD, ActionType.ADD),
                    TestUtils.CreateTrade(10, CurrencyType.FOOD, ActionType.REMOVE),
                    TestUtils.CreateTrade(10, CurrencyType.WOOD, ActionType.REMOVE)
                ];

            _updateDTOs = _trades.Select(trade => new CurrencyUpdateDTO 
                {
                    Amount = trade.Amount,
                    Currency = trade.Currency,
                    Action = trade.Action
                }).ToList();
        }

        [Test]
        public void Positive_Dispatch_DispatchesDTO()
        {
            _currencyUpdateFactoryMock.Setup(library => library.CreateFrom(_trades))
                .Returns(_updateDTOs);
            
            _bufferManagerMock.Setup(library => library.RequestBuffer<CurrencyUpdateDTO>(It.IsAny<BufferRequest>()))
                .Returns(_bufferMock.Object);
            
            _currencyDispatcher.Dispatch(_trades);
            
            _bufferManagerMock.Verify(library => library.RequestBuffer<CurrencyUpdateDTO>(new BufferRequest(4)));
            _bufferMock.Verify(library => library.Assign(It.IsAny<CurrencyUpdateDTO[]>()));
            _bufferMock.Verify(library => library.MarkReady());
        }

        [Test]
        public void Negative_Dispatch_NegativeNumberInCollection_Throws()
        {
            IReadOnlyList<CurrencyTrade> trades =
            [
                new()
                {
                    Action = ActionType.REMOVE,
                    Amount = 10,
                    Currency = CurrencyType.FOOD
                }, 
                new()
                {
                    Action = ActionType.ADD,
                    Amount = -1,
                    Currency = CurrencyType.FOOD
                }
            ];
            
            _asserterMock.Setup(library => library.AssertTradeCollection(trades))
                    .Throws(new NegativeNumberException(-1));
            
            
            Assert.Throws<NegativeNumberException>(() => _currencyDispatcher.Dispatch(trades));
            _bufferManagerMock.Verify(library => library.RequestBuffer<CurrencyUpdateDTO>(new BufferRequest(1)), Times.Never);
            _bufferMock.Verify(library => library.Assign(It.IsAny<CurrencyUpdateDTO[]>()), Times.Never);
            _bufferMock.Verify(library => library.MarkReady(), Times.Never);
        }

        [Test]
        public void Negative_Dispatch_NullCollection_Throws()
        {
            _asserterMock.Setup(library => library.AssertTradeCollection(null!))
                .Throws<ArgumentNullException>();

            Assert.Throws<ArgumentNullException>(() => _currencyDispatcher.Dispatch(null!));
            _bufferManagerMock.Verify(library => library.RequestBuffer<CurrencyUpdateDTO>(new BufferRequest(1)), Times.Never);
            _bufferMock.Verify(library => library.Assign(It.IsAny<CurrencyUpdateDTO[]>()), Times.Never);
            _bufferMock.Verify(library => library.MarkReady(), Times.Never);
        }

        [Test]
        public void Negative_Dispatch_EmptyCollection_Throws()
        {
            IReadOnlyList<CurrencyTrade> trades = [];
            
            _asserterMock.Setup(library => library.AssertTradeCollection(trades))
                .Throws<CollectionEmptyException>();
            
            Assert.Throws<CollectionEmptyException>(() => _currencyDispatcher.Dispatch(trades));
            _bufferManagerMock.Verify(library => library.RequestBuffer<CurrencyUpdateDTO>(new BufferRequest(1)), Times.Never);
            _bufferMock.Verify(library => library.Assign(It.IsAny<CurrencyUpdateDTO[]>()), Times.Never);
            _bufferMock.Verify(library => library.MarkReady(), Times.Never);
        }
    }
}