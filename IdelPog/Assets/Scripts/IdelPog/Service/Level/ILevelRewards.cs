using IdelPog.Structures;

namespace IdelPog.Service.Level
{
    /// <summary>
    /// Handles granting custom rewards which can be granted on specific numbers
    /// </summary>
    /// <seealso cref="MaybeGrantReward"/>
    public interface ILevelRewards
    {
        /// <summary>
        /// Will grant an award if the passed level matches amy <see cref="LevelAward"/> passed on construct
        /// </summary>
        /// <param name="level">The level you want to maybe grant an award for</param>
        public void MaybeGrantReward(byte level);
    }
}