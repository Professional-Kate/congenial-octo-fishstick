using IdelPog.Engine.Structures.Models.Levelable;

namespace IdelPog.Engine.Service.Level
{
    /// <inheritdoc cref="ILevelRewards"/>
    public class LevelRewards : ILevelRewards
    {
        private readonly LevelAward[] _sortedRewards;
        private byte _nextLevelRewardIndex;

        public LevelRewards(LevelAward[] rewards)
        {
            _sortedRewards = rewards.OrderBy(levelAward => levelAward.RequiredLevel).ToArray();
        }
        
        public void MaybeGrantReward(byte level)
        {
            if (_sortedRewards.Length <= _nextLevelRewardIndex)
            {
                return;
            }
            
            LevelAward nextLevelAward = _sortedRewards[_nextLevelRewardIndex];
            if (level < nextLevelAward.RequiredLevel)
            {
                return;
            }

            while (level >= nextLevelAward.RequiredLevel)
            {
                nextLevelAward.OnLevelUp();
                
                _nextLevelRewardIndex++;
                if (_nextLevelRewardIndex >= _sortedRewards.Length)
                {
                    break;
                }
                
                nextLevelAward = _sortedRewards[_nextLevelRewardIndex];
            }
        }
    }
}