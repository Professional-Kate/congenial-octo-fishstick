namespace IdelPog.SimulationEngine.Currency.Exceptions
{
    public class NegativeNumberException : Exception
    {
        private const string MESSAGE = "The passed {0} contains a negative number! This is not allowed, we are positive here...";

        public readonly Type NumberSource;

        public NegativeNumberException(Type numberSource) : base(string.Format(MESSAGE, numberSource.Name))
        {
            NumberSource = numberSource;
        }
    }
}