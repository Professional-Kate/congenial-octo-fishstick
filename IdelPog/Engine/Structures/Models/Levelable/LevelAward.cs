namespace IdelPog.Engine.Structures.Models.Levelable
{
    /// <seealso cref="RequiredLevel"/>
    /// <seealso cref="OnLevelUp"/>
    public class LevelAward
    {
        public readonly byte RequiredLevel;
        public readonly Action OnLevelUp;

        public LevelAward(byte level, Action onLevelUp)
        {
            RequiredLevel = level;
            OnLevelUp = onLevelUp;
        }
    }
}