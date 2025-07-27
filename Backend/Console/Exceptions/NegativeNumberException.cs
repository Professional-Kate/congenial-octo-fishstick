namespace Console.Exceptions
{
    public class NegativeNumberException : Exception
    {
        private const string MESSAGE = "The passed number: {0} is negative! This is not allowed, we are positive here...";

        public readonly int Number;

        public NegativeNumberException(int number) : base(string.Format(MESSAGE, number))
        {
            Number = number;
        }
    }
}