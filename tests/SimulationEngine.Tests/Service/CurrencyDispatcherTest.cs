using IdelPog.Messaging.Collection;
using IdelPog.Messaging.Orchestration;
using IdelPog.SimulationEngine.Flows.Currency;
using IdelPog.SimulationEngine.Structures.Enums;
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
        private Mock<IBuffer<CurrencyUpdateDTO>> _bufferMock { get; set; }
        
        private IReadOnlyList<CurrencyTrade> _trades { get; set; }
        private IReadOnlyList<CurrencyUpdateDTO> _updateDTOs { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _bufferMock = new Mock<IBuffer<CurrencyUpdateDTO>>();
            _bufferManagerMock = new Mock<IBufferManager>();
            _currencyUpdateFactoryMock = new Mock<ICurrencyUpdateFactory>();
            _currencyDispatcher = new CurrencyDispatcher(_bufferManagerMock.Object,  _currencyUpdateFactoryMock.Object);

            CreateTestObjects();
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
            
            _bufferManagerMock.Verify(library => library.RequestBuffer<CurrencyUpdateDTO>(new BufferRequest(1)));
            _bufferMock.Verify(library => library.Assign(It.IsAny<CurrencyUpdateDTO[]>()));
            _bufferMock.Verify(library => library.MarkReady());
        }
    }
}