using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Core.Tests.Validation.Assertion
{
    [TestFixture]
    public sealed class CollectionAssertionTest
    {
        private ICollectionAssertion _assertion;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _assertion = new CollectionAssertion();
        }

        [Test]
        public void Positive_AssertNotNull_NotNullCollection_NoThrow()
        {
            Assert.DoesNotThrow(() => _assertion.AssertNotNull(new List<int>()));
        }

        [Test]
        public void Negative_AssertNotNull_NullCollection_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _assertion.AssertNotNull<int>(null!));
        }

        [Test]
        public void Positive_AssertNotEmpty_NotEmptyCollection_NoThrow()
        {
            Assert.DoesNotThrow(() => _assertion.AssertNotEmpty(new List<int> { 1, 2, 3 }));
        }

        [Test]
        public void Negative_AssertNotEmpty_EmptyCollection_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _assertion.AssertNotEmpty(new List<int>()));
        }

        [Test]
        public void Positive_AssertHasElements_CollectionHasElements_NoThrow()
        {
            Assert.DoesNotThrow(() => _assertion.AssertHasElements(new List<int> { 1, 2, 3 }));
        }

        [Test]
        public void Negative_AssertHasElements_NullCollection_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _assertion.AssertHasElements<int>(null));
        }

        [Test]
        public void Negative_AssertHasElements_EmptyCollection_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _assertion.AssertHasElements(new List<int>()));
        }
    }
}