using IdelPog.Structures.Builders;
using IdelPog.Structures.Models.Levelable;
using IdelPog.Validation;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Interfaces;
using NUnit.Framework;

namespace Tests.Validation
{
    [TestFixture]
    public class AssertUnderMaxLevelTest
    {
        private IAssertUnderMaxLevel _assertUnderMaxLevel { get; set; }
        private ILevelable _levelable { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _levelable = LevelableBuilder.Builder()
                .Level(0)
                .Experience(1)
                .NextLevelExperience(10)
                .ExperiencePerAction(0)
                .Build();
            
            _assertUnderMaxLevel = new AssertUnderMaxLevel(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertLevelIsUnderMax_LevelUnderMax()
        {
            _levelable = LevelableBuilder.Builder()
                .Level(99)
                .Experience(1)
                .NextLevelExperience(10)
                .ExperiencePerAction(0)
                .Build();
            
            Assert.DoesNotThrow(() => _assertUnderMaxLevel.AssertLevelIsUnderMax(_levelable));
        }

        [Test]
        public void Negative_AssertLevelIsUnderMax_LevelIsMax_Throws()
        {
            _levelable = LevelableBuilder.Builder()
                .Level(100)
                .Experience(1)
                .NextLevelExperience(1)
                .ExperiencePerAction(1)
                .Build();
            
            Assert.Throws<MaxLevelException>(() => _assertUnderMaxLevel.AssertLevelIsUnderMax(_levelable));
        }
    }
}