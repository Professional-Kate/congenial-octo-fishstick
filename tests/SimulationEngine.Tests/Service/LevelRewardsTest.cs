using IdelPog.SimulationEngine.Constants;
using IdelPog.SimulationEngine.Service;
using IdelPog.SimulationEngine.Structures.Types;

namespace IdelPogTests.Service
{
    [TestFixture]
    public class LevelRewardsTest
    {
        private ILevelRewards _levelRewards { get; set; }

        private LevelAward[] _levelAwards { get; set; }
        private const byte LEVELS_PER_AWARD = 5;
        
        private bool _wasOnLevelUpCalled;
        private int _levelUpCalledAmount;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            SetupLevelAwards();
        }

        [SetUp]
        public void SetUp()
        {
            _levelRewards = new LevelRewards(_levelAwards);
        }

        [TearDown]
        public void TearDown()
        {
            _wasOnLevelUpCalled = false;
            _levelUpCalledAmount = 0;
        }

        private void SetupLevelAwards()
        {
            List<LevelAward> levelAwards = new();
            
            for (byte i = LEVELS_PER_AWARD; i <= SkillConstants.MAX_SKILL_LEVEL; i += LEVELS_PER_AWARD)
            {
                LevelAward levelAward = new(i, OnLevelUp);
                levelAwards.Add(levelAward);
            } 
            
            levelAwards.Reverse();
            _levelAwards = levelAwards.ToArray();
        }

        private void OnLevelUp()
        {
            _wasOnLevelUpCalled = true;
            _levelUpCalledAmount++;
        }

        [Test]
        public void Positive_MaybeGrantReward_GrantsReward()
        {
            _levelRewards.MaybeGrantReward(5);
            
            Assert.That(_wasOnLevelUpCalled, Is.True);
            Assert.That(1, Is.EqualTo(_levelUpCalledAmount));
        }

        [Test]
        public void Positive_MaybeGrantReward_GrantsRewardEveryLevel()
        {
            for (byte i = LEVELS_PER_AWARD; i <= SkillConstants.MAX_SKILL_LEVEL; i += LEVELS_PER_AWARD)
            {
                _levelRewards.MaybeGrantReward(i);
            }

            Assert.That(_wasOnLevelUpCalled, Is.True);
            Assert.That(20, Is.EqualTo(_levelUpCalledAmount));
        }

        [Test]
        public void Positive_MaybeGrantReward_GrantsMultipleMissedRewards()
        {
            _levelRewards.MaybeGrantReward(23);
            
            Assert.That(_wasOnLevelUpCalled, Is.True);
            Assert.That(4, Is.EqualTo(_levelUpCalledAmount));
        }

        [Test]
        public void Positive_MaybeGrantReward_GrantsReward_OnlyOnce()
        {
            _levelRewards.MaybeGrantReward(5);
            _levelRewards.MaybeGrantReward(5);
            
            Assert.That(_wasOnLevelUpCalled, Is.True);
            Assert.That(1, Is.EqualTo(_levelUpCalledAmount));
        }

        [Test]
        public void Positive_MaybeGrantReward_EmptyArray()
        {
            LevelAward[] levelAwards = Array.Empty<LevelAward>();
            ILevelRewards levelRewards = new LevelRewards(levelAwards);
            
            levelRewards.MaybeGrantReward(10);
            Assert.That(_wasOnLevelUpCalled, Is.False);
        }

        [Test]
        public void Negative_MaybeGrantReward_DoesNotGrantReward()
        {
            _levelRewards.MaybeGrantReward(4);
            
            Assert.That(_wasOnLevelUpCalled, Is.False);
            Assert.That(0, Is.EqualTo(_levelUpCalledAmount));
        }
    }
}