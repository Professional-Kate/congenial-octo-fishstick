using IdelPog.Combat.Assertion;
using IdelPog.Combat.Exceptions;

namespace IdelPog.Combat.Tests.Assertion
{
    [TestFixture]
    public sealed class NumberAssertionTest
    {
        private NumberAssertion _numberAssertion;
        private const string SOURCE = "test";

        [OneTimeSetUp]
        public void OneTimeSetup()
        { 
            _numberAssertion = new NumberAssertion();
        }

        [Test]
        public void Positive_AssertNumberNotZero_NumberNotZero_NoThrow()
        {
            Assert.DoesNotThrow(() => _numberAssertion.AssertNumberNotZero(1, SOURCE));
        }
        
        [Test]
        public void Negative_AssertNumberNotZero_NumberZero_Throws()
        {
            NumberZeroException exception = Assert.Throws<NumberZeroException>(() => _numberAssertion.AssertNumberNotZero(0, SOURCE));
            
            Assert.That(exception.Source, Is.EqualTo(SOURCE));
        }
    }
}