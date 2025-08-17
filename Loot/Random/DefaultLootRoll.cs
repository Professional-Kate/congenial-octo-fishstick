namespace IdelPog.Loot.Random
{
    public sealed class DefaultLootRoll : ILootRoll
    {
        private readonly System.Random _random;

        public DefaultLootRoll()
        {
            _random = new System.Random();
        }
        
        public DefaultLootRoll(int seed)
        {
            _random = new System.Random(seed);
        }
        
        public uint ExclusiveNextInt(uint minInclusive, uint maxExclusive)
        {
            int value = _random.Next((int) minInclusive, (int) maxExclusive);
            return (uint) value;
        }
    }
}