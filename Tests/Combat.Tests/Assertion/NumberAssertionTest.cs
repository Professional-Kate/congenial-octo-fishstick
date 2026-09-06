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
            Assert.DoesNotThrow(() => _numberAssertion.AssertNumberNotZeroOrNegative(1D, SOURCE));
        }
        
        [Test]
        public void Negative_AssertNumberNotZero_NumberZero_Throws()
        {
            NumberZeroException exception = Assert.Throws<NumberZeroException>(() => _numberAssertion.AssertNumberNotZero(0, SOURCE));
            Assert.That(exception.Source, Is.EqualTo(SOURCE));
            
            exception = Assert.Throws<NumberZeroException>(() => _numberAssertion.AssertNumberNotZeroOrNegative(0d, SOURCE));
            Assert.That(exception.Source, Is.EqualTo(SOURCE));
        }

        [Test]
        public void Negative_AssertNumberNotZero_Double_Throws()
        {
            Assert.Throws<NumberZeroException>(() => _numberAssertion.AssertNumberNotZero(0d, SOURCE));
            Assert.DoesNotThrow(() => _numberAssertion.AssertNumberNotZero(-1d, SOURCE));
        }

        [Test]
        public void Positive_AssertNumberNotZeroOrNegative_NoThrow()
        { 
            Assert.DoesNotThrow(() => _numberAssertion.AssertNumberNotZeroOrNegative(1D, SOURCE));
        }

        [Test]
        public void Negative_AssertNumberNotZeroOrNegative_Throws()
        {
            NumberZeroException numberZeroException = Assert.Throws<NumberZeroException>(() => _numberAssertion.AssertNumberNotZeroOrNegative(0D, SOURCE));
            Assert.That(numberZeroException.Source, Is.EqualTo(SOURCE));
            
            NegativeNumberException negativeNumberException = Assert.Throws<NegativeNumberException>(() => _numberAssertion.AssertNumberNotZeroOrNegative(-1D, SOURCE));
            Assert.That(negativeNumberException.Source, Is.EqualTo(SOURCE));
        }
    }
}