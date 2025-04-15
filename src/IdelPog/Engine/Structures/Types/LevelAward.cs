namespace IdelPog.Engine.Structures.Types
{
    /// <seealso cref="RequiredLevel"/>
    /// <seealso cref="OnLevelUp"/>
    public class LevelAward(byte level, Action onLevelUp)
    {
        public readonly byte RequiredLevel = level;
        public readonly Action OnLevelUp = onLevelUp;
    }
}