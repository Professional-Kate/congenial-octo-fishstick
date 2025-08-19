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
        
        public int ExclusiveNextInt(int minInclusive, int maxExclusive)
        {
            int value = _random.Next(minInclusive, maxExclusive);
            return value;
        }
    }
}