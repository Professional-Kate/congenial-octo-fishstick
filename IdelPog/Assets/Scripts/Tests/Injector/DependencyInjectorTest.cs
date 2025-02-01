using Injector;
using Injector.Attributes;
using NUnit.Framework;

namespace Tests.Injector
{
    [TestFixture]
    public class DependencyInjectorTest
    {
        private DependencyInjector _injector { get; set; }
        [Inject] private TestComponent _testComponent { get; set; }

        [SetUp]
        public void SetUp()
        {
            _injector = DependencyInjector.GetInstance();
        }

        [Test]
        public void Positive_Get_ReturnsComponent()
        {
            TestComponent component = _injector.Get<TestComponent>();
            
            Assert.IsNotNull(component);
            Assert.IsInstanceOf<TestComponent>(component);
        }
    }
}