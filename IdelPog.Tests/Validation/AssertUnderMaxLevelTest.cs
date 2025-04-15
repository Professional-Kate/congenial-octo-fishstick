using IdelPog.Engine.Assertions;
using IdelPog.Engine.Models;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Exceptions;

namespace IdelPogTests.Validation
{
    [TestFixture]
    public class AssertUnderMaxLevelTest
    {
        private IAssertUnderMaxLevel _assertUnderMaxLevel { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _assertUnderMaxLevel = new AssertUnderMaxLevel(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertLevelIsUnderMax_LevelUnderMax()
        {
            ILevelable levelable = new Levelable(99, 1, 10, 0);
            
            Assert.DoesNotThrow(() => _assertUnderMaxLevel.AssertLevelIsUnderMax(levelable));
        }

        [Test]
        public void Negative_AssertLevelIsUnderMax_LevelIsMax_Throws()
        {
            ILevelable levelable = new Levelable(100, 1, 1, 1);
            
            Assert.Throws<MaxLevelException>(() => _assertUnderMaxLevel.AssertLevelIsUnderMax(levelable));
        }
    }
}