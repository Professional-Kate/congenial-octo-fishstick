namespace IdelPog.Combat.Exceptions
{
    public sealed class NumberZeroException : Exception
    {
        private const string MESSAGE = "Silly!! The passed structure contains a zero number, this is not allowed!\n{0}\n This is cringe!!!";

        public NumberZeroException(string source) : base(string.Format(MESSAGE, source))
        {
            Source = source;
        }
    }
}