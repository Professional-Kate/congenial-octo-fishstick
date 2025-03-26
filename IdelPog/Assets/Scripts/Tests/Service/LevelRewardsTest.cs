using System.Collections.Generic;
using IdelPog.Constants;
using IdelPog.Structures;
using NUnit.Framework;

namespace Tests.Service
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
            
            for (byte i = LEVELS_PER_AWARD; i <= JobConstants.MAX_JOB_LEVEL; i += LEVELS_PER_AWARD)
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
            
            Assert.IsTrue(_wasOnLevelUpCalled);
            Assert.AreEqual(1, _levelUpCalledAmount);
        }

        [Test]
        public void Positive_MaybeGrantReward_GrantsRewardEveryLevel()
        {
            for (byte i = LEVELS_PER_AWARD; i <= JobConstants.MAX_JOB_LEVEL; i += LEVELS_PER_AWARD)
            {
                _levelRewards.MaybeGrantReward(i);
            }

            Assert.IsTrue(_wasOnLevelUpCalled);
            Assert.AreEqual(20, _levelUpCalledAmount);
        }

        [Test]
        public void Positive_MaybeGrantReward_GrantsMultipleMissedRewards()
        {
            _levelRewards.MaybeGrantReward(100);
            
            Assert.IsTrue(_wasOnLevelUpCalled);
            Assert.AreEqual(20, _levelUpCalledAmount);
        }

        [Test]
        public void Positive_MaybeGrantReward_GrantsReward_OnlyOnce()
        {
            _levelRewards.MaybeGrantReward(5);
            _levelRewards.MaybeGrantReward(5);
            
            Assert.IsTrue(_wasOnLevelUpCalled);
            Assert.AreEqual(1, _levelUpCalledAmount);
        }

        [Test]
        public void Negative_MaybeGrantReward_DoesNotGrantReward()
        {
            _levelRewards.MaybeGrantReward(4);
            
            Assert.IsFalse(_wasOnLevelUpCalled);
            Assert.AreEqual(0, _levelUpCalledAmount);
        }
    }
}