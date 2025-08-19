namespace IdelPog.Loot.Random
{
    public interface ILootRoll
    {
        public int ExclusiveNextInt(int minInclusive, int maxExclusive);
    }
}