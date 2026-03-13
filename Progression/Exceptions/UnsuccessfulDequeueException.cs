namespace IdelPog.Progression.Exceptions
{
    public sealed class UnsuccessfulDequeueException : Exception
    {
        private const string MESSAGE = "Could not dequeue component!";

        public UnsuccessfulDequeueException() : base(string.Format(MESSAGE))
        {
        }
    }
}