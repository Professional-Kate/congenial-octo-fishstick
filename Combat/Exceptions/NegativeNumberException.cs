namespace IdelPog.Combat.Exceptions
{
    public sealed class NegativeNumberException : Exception
    {
        private const string MESSAGE = "Silly!!! This {0} number is negative!! How could you be so cringe? One of the ONLY doubles that you can pass in and you pass in a NEGATIVE";
        
        public NegativeNumberException(string source) : base(string.Format(MESSAGE, source))
        {
            Source = source;
        }
    }
}