namespace IdelPog.Loot.Exceptions
{
    public class ZeroWeightException : Exception
    {
        private const string MESSAGE = "The total weight of the WeightedEntry's is zero!! Why did you do this...";
        
        public ZeroWeightException() : base(MESSAGE) { }
    }
}