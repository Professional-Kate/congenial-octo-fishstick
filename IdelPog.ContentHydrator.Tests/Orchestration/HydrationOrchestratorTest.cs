using ContentHydrator.Orchestration;
using ContentHydrator.Service;
using Moq;

namespace ContentHydratorTests.Orchestration
{
    [TestFixture]
    public class HydrationOrchestratorTest
    {
        private IHydrationOrchestrator _hydrationOrchestrator { get; set; }
        private Mock<IDirectoryConverter> _directoryConverterMock { get; set; }
            
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _directoryConverterMock = new Mock<IDirectoryConverter>();
            _hydrationOrchestrator = new HydrationOrchestrator(_directoryConverterMock.Object);
        }

        [Test]
        public void Positive_Hydrate_HydratesTestObjects()
        {
            
        }
    }
}