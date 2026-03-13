namespace IdelPog.Progression.Exceptions
{
    public sealed class UnsuccessfulPeekException : Exception
    {
        private const string MESSAGE = "Could not Peek!";

        public UnsuccessfulPeekException() : base(string.Format(MESSAGE))
        {
        }
    }
}