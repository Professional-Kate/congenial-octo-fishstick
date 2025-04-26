using ContentHydrator.Assertions;
using ContentHydrator.Exceptions;
using IdelPog.Validation.Assertions.Handlers;

namespace ContentHydratorTests.Assertions
{
    [TestFixture]
    public class AssertDirectoryNotEmptyTest
    {
        private IAssertDirectoryNotEmpty _assertDirectoryNotEmpty { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _assertDirectoryNotEmpty = new AssertDirectoryNotEmpty(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertNotEmpty_NonEmptyArray()
        {
            string[] items = ["a"];
            
            Assert.DoesNotThrow(() => _assertDirectoryNotEmpty.AssertNotEmpty(items, "TEST"));
        }

        [Test]
        public void Negative_AssertNotEmpty_EmptyArray_Throws()
        {
            Assert.Throws<EmptyDirectoryException>(() => _assertDirectoryNotEmpty.AssertNotEmpty([], "TEST"));
        }
    }
}