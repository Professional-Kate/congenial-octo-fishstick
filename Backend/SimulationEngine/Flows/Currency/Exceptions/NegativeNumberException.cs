namespace IdelPog.SimulationEngine.Currency.Exceptions
{
    public class NegativeNumberException : Exception
    {
        private const string MESSAGE = "The passed number is negative! This is not allowed, we are positive here...";

        public NegativeNumberException() : base(string.Format(MESSAGE))
        {
        }
    }
}