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
        private Mock<IBuffer<CurrencyUpdateDTO>> _bufferMock { get; set; }
        private CurrencyTrade _trade { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _bufferMock = new Mock<IBuffer<CurrencyUpdateDTO>>();
            _bufferManagerMock = new Mock<IBufferManager>();
            _currencyDispatcher = new CurrencyDispatcher(_bufferManagerMock.Object);
            _trade = TestUtils.CreateTrade(10, CurrencyType.FOOD, ActionType.ADD);
        }

        [Test]
        public void Positive_Dispatch_DispatchesDTO()
        {
            _bufferManagerMock.Setup(library => library.RequestBuffer<CurrencyUpdateDTO>(It.IsAny<BufferRequest>()))
                .Returns(_bufferMock.Object);
            
            _currencyDispatcher.Dispatch(_trade);
            
            _bufferManagerMock.Verify(library => library.RequestBuffer<CurrencyUpdateDTO>(new BufferRequest(1)));
            _bufferMock.Verify(library => library.Assign(It.IsAny<CurrencyUpdateDTO[]>()));
            _bufferMock.Verify(library => library.MarkReady());
        }
    }
}