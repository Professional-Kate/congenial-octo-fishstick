using IdelPog.Structures.Builders;
using IdelPog.Structures.Models;
using IdelPog.Structures.Models.Levelable;
using Moq;
using NUnit.Framework;

namespace Tests.Models
{
    [TestFixture]
    public class LevelableTest
    {
        private ILevelable _levelable { get; set; }
        private Mock<ILevelRewards> _levelRewardsMock { get; set; }

        private const byte LEVEL = 5;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _levelRewardsMock = new Mock<ILevelRewards>();
           
            _levelable = LevelableBuilder.Builder()
                .Level(LEVEL)
                .LevelRewards(_levelRewardsMock.Object)
                .Build();
        }

        [Test]
        public void Positive_LevelUp_CallsLevelRewards()
        {
            _levelable.LevelUp();
            const byte newLevel = LEVEL + 1;
            
            _levelRewardsMock.Verify(library => library.MaybeGrantReward(newLevel), Times.Once());
        }
    }
}