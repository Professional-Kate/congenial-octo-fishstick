namespace IdelPog.Loot.Exceptions
{
    public sealed class InvalidWeightException : Exception
    {
        private const string MESSAGE = "The total weight of the WeightedEntry's is zero!! Why did you do this...";
        
        public InvalidWeightException() : base(MESSAGE) { }
    }
}