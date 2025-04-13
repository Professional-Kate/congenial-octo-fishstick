using IdelPog.Engine.Structures.Models;
using IdelPog.Engine.Validation.Assertions;
using IdelPog.Engine.Validation.Assertions.Handlers;
using IdelPog.Engine.Validation.Exceptions;
using IdelPog.Engine.Validation.Pipelines;

namespace IdelPogTests.Validation.Pipelines
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
            
            Assert.Throws<NegativeNumberException>(() => _levelableAsserter.AssertLevelable(levelable));
        }
    }
}