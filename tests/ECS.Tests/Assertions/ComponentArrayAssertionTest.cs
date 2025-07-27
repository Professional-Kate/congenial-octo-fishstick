using IdelPog.ECS.Assertions;
using IdelPog.ECS.Exceptions;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.ECS.Tests.Assertions
{
    [TestFixture]
    public class ComponentArrayAssertionTest
    {
        private IComponentArrayAssertion _assertion;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _assertion = new ComponentArrayAssertion(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertNotNull_NotNullArray_NoThrow()
        {
            Assert.DoesNotThrow(() => _assertion.AssertNotNull([1, 2, 3]));
        }

        [Test]
        public void Negative_AssertNotNull_NullArray_Throws()
        {
            Assert.Throws<ComponentArrayNullException>(() => _assertion.AssertNotNull<int>(null!));
        }

        [Test]
        public void Positive_AssertNotEmpty_NonEmptyArray_NoThrow()
        {
            Assert.DoesNotThrow(() => _assertion.AssertNotEmpty([1, 2, 3]));
        }

        [Test]
        public void Negative_AssertNotEmpty_EmptyArray_Throws()
        {
            Assert.Throws<ComponentArrayEmptyException>(() => _assertion.AssertNotEmpty<int>([]));
        }

        [Test]
        public void Positive_AssertHasElements_ArrayHasElement_NoThrow()
        {
            Assert.DoesNotThrow(() => _assertion.AssertHasElements([1, 2, 3]));
        }

        [Test]
        public void Negative_AssertHasElements_ArrayHasNoElements_Throws()
        {
            Assert.Throws<ComponentArrayEmptyException>(() => _assertion.AssertHasElements<int>([]));
        }

        [Test]
        public void Negative_AssertHasElements_NullArray_Throws()
        {
            Assert.Throws<ComponentArrayNullException>(() => _assertion.AssertHasElements<int>(null!));
        }
    }
}