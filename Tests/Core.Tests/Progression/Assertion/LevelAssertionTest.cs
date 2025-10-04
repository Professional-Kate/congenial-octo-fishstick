using IdelPog.Core.Progression;
using IdelPog.Core.Progression.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;

namespace IdelPog.Core.Tests.Progression.Assertion
{
    [TestFixture]
    public class LevelAssertionTest
    {
        private ILevelAssertion _levelAssertion { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _levelAssertion = new LevelAssertion(new ThrowHandler());
        }

        private static void AssertException(MaxLevelException maxLevelException, Levelable levelable)
        {
            Assert.Multiple(() =>
            {
                Assert.That(maxLevelException.ID, Is.EqualTo(levelable));
                Assert.That(maxLevelException.SourceName, Is.EqualTo(nameof(levelable)));
            });
        }

        [Test]
        public void Positive_AssertBelowMaxLevel_LevelUnderMax_NoThrow()
        {
            Levelable levelable = new(LevelConstants.MAX_LEVEL - 1, 1, 10, 0);

            Assert.DoesNotThrow(() => _levelAssertion.AssertBelowMaxLevel(levelable));
        }

        [Test]
        public void Negative_AssertBelowMaxLevel_LevelIsMax_Throws()
        {
            Levelable levelable = new(LevelConstants.MAX_LEVEL, 1, 1, 1);

            MaxLevelException exception = Assert.Throws<MaxLevelException>(() => _levelAssertion.AssertBelowMaxLevel(levelable));
            AssertException(exception, levelable);
        }

        [Test]
        public void Negative_AssertBelowMaxLevel_LevelAboveMax_Throws()
        {
            Levelable levelable = new(LevelConstants.MAX_LEVEL + 1, 1, 1, 1);
            
            MaxLevelException exception = Assert.Throws<MaxLevelException>(() => _levelAssertion.AssertBelowMaxLevel(levelable));
            AssertException(exception, levelable);
        }

        [Test]
        public void Positive_AssertNotAboveMaxLevel_LevelUnderMax_NoThrow()
        {
            Levelable levelable = new(LevelConstants.MAX_LEVEL - 1, 1, 10, 0);

            Assert.DoesNotThrow(() => _levelAssertion.AssertNotAboveMaxLevel(levelable));
        }
        
        [Test]
        public void Positive_AssertNotAboveMaxLevel_LevelIsMax_NoThrow()
        {
            Levelable levelable = new(LevelConstants.MAX_LEVEL, 1, 10, 0);

            Assert.DoesNotThrow(() => _levelAssertion.AssertNotAboveMaxLevel(levelable));
        }

        [Test]
        public void Negative_AssertNotAboveMaxLevel_LevelAboveMax_Throws()
        {
            Levelable levelable = new(LevelConstants.MAX_LEVEL + 1, 1, 1, 1);
            
            MaxLevelException exception = Assert.Throws<MaxLevelException>(() => _levelAssertion.AssertNotAboveMaxLevel(levelable));
            AssertException(exception, levelable);
        }
    }
}