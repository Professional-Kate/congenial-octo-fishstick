using IdelPog.SimulationEngine.Assertions;
using IdelPog.SimulationEngine.Assertions.Pipelines;
using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Exceptions;
using IdelPog.SimulationEngine.Models;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Exceptions;

namespace IdelPogTests.Assertions.Pipelines
{
    [TestFixture]
    public class LevelableAsserterTest
    {
        private ILevelableAsserter _levelableAsserter { get; set; }
        private ILevelable _levelable { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _levelable = new Levelable(1, 0, 10, 0);

            IHandler handler = new ThrowHandler();
            IAssertUnderMaxLevel assertUnderMaxLevel = new AssertUnderMaxLevel(handler);
            IAssertNotNull assertNotNull = new AssertNotNull(handler);
            IAssertPositive assertPositive = new AssertPositive(handler);

            _levelableAsserter = new LevelableAsserter(assertUnderMaxLevel, assertNotNull, assertPositive);
        }

        [Test]
        public void Positive_AssertLevelable_LevelableGood()
        {
            Assert.DoesNotThrow(() => _levelableAsserter.AssertLevelable(_levelable));
        }

        [Test]
        public void Negative_AssertLevelable_NullLevelable_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _levelableAsserter.AssertLevelable(null));
        }

        [Test]
        public void Negative_AssertLevelable_MaxLevel_Throws()
        {
            ILevelable levelable = new Levelable(100, 0, 10, 0);

            Assert.Throws<MaxLevelException>(() => _levelableAsserter.AssertLevelable(levelable));
        }

        [Test]
        public void Positive_AssertLevelable_NegativeExperiencePerAction_Throws()
        {
            ILevelable levelable = new Levelable(0, 0, 0, -1);

            NegativeNumberException exception = Assert.Throws<NegativeNumberException>(() => _levelableAsserter.AssertLevelable(levelable));
            Assert.That(exception.NumberSource, Is.EqualTo(typeof(ILevelable)));
        }
    }
}