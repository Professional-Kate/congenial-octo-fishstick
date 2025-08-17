namespace IdelPog.Loot.Random
{
    public interface ILootRoll
    {
        public uint ExclusiveNextInt(uint minInclusive, uint maxExclusive);
    }
}