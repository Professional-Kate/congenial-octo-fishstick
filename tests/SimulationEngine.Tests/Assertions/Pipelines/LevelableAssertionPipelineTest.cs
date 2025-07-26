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
    public class LevelableAssertionPipelineTest
    {
        private ILevelableAssertionPipeline _levelableAssertionPipeline { get; set; }
        private ILevelable _levelable { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _levelable = new Levelable(1, 0, 10, 0);

            IHandler handler = new ThrowHandler();
            ILevelAssertion levelAssertion = new LevelAssertion(handler);
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(handler);
            INumberAssertion numberAssertion = new NumberAssertion(handler);

            _levelableAssertionPipeline = new LevelableAssertionPipeline(levelAssertion, objectNullAssertion, numberAssertion);
        }

        [Test]
        public void Positive_AssertLevelable_LevelableGood()
        {
            Assert.DoesNotThrow(() => _levelableAssertionPipeline.AssertLevelable(_levelable));
        }

        [Test]
        public void Negative_AssertLevelable_NullLevelable_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _levelableAssertionPipeline.AssertLevelable(null!));
        }

        [Test]
        public void Negative_AssertLevelable_MaxLevel_Throws()
        {
            ILevelable levelable = new Levelable(100, 0, 10, 0);

            Assert.Throws<MaxLevelException>(() => _levelableAssertionPipeline.AssertLevelable(levelable));
        }

        [Test]
        public void Positive_AssertLevelable_NegativeExperiencePerAction_Throws()
        {
            ILevelable levelable = new Levelable(0, 0, 0, -1);

            Assert.Throws<NegativeNumberException>(() => _levelableAssertionPipeline.AssertLevelable(levelable));
        }
    }
}