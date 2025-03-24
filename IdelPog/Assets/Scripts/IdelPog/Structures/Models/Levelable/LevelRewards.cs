using System;
using System.Linq;

namespace IdelPog.Structures
{
    /// <inheritdoc cref="ILevelRewards"/>
    public class LevelRewards : ILevelRewards
    {
        private readonly LevelAward[] _sortedRewards;

        public LevelRewards(params LevelAward[] rewards)
        {
            _sortedRewards = rewards.OrderBy(levelAward => levelAward.RequiredLevel).ToArray();
        }
        
        public void MaybeGrantReward(byte level)
        {
            throw new NotImplementedException();
        }
    }
}