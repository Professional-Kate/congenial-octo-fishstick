using IdelPog.ECS.Assertions;
using IdelPog.ECS.Exceptions;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.ECS.Tests.Assertions
{
    [TestFixture]
    public class ComponentAssertionTest
    {
        private IComponentAssertion _assertion;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _assertion = new ComponentAssertion(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertUnique_PassesFalse_NoThrow()
        {
            Assert.DoesNotThrow(() => _assertion.AssertUnique<int>(false));
        }

        [Test]
        public void Negative_AssertUnique_PassesTrue_Throws()
        {
            Assert.Throws<ComponentAlreadyExistsException>(() => _assertion.AssertUnique<int>(true));
        }

        [Test]
        public void Positive_AssertFound_PassesTrue_NoThrow()
        {
            Assert.DoesNotThrow(() => _assertion.AssertFound<int>(true));
        }

        [Test]
        public void Negative_AssertFound_PassesFalse_Throws()
        {
            Assert.Throws<ComponentNotFoundException>(() => _assertion.AssertFound<int>(false));
        }
    }
}