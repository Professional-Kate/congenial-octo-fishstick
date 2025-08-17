namespace IdelPog.Loot.Contracts
{
    public interface ILootRoll
    {
        public uint ExclusiveNextInt(uint minInclusive, uint maxExclusive);
    }
}