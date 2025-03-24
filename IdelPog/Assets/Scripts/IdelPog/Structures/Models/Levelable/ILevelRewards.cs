namespace IdelPog.Structures
{
    public interface ILevelRewards
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        public void MaybeGrantReward(byte level);
    }
}