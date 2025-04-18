using ContentHydrator.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace ContentHydratorTests.Assertions
{
    [TestFixture]
    public class AssertValidCastTest
    {
        private IAssertValidCast _assertValidCast { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _assertValidCast = new AssertValidCast(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertCastable_GoodCast()
        {
            object value = "Hello, I am a string";
            
            _assertValidCast.AssertCastable<string>(value);
        }

        [Test]
        public void Negative_AssertCastable_BadCast_Throws()
        {
            object value = "Hello, I am not a string";
            
            Assert.Throws<InvalidCastException>(() => _assertValidCast.AssertCastable<int>(value));
        }
    }
}