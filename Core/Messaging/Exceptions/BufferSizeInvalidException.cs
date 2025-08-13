namespace IdelPog.Core.Messaging.Exceptions
{
    public class BufferSizeInvalidException : Exception
    {
        private const string MESSAGE = "The passed collection size is 0 or negative! {0} is not valid!!!!";

        public readonly int Size;

        public BufferSizeInvalidException(int size) : base(string.Format(MESSAGE, size))
        {
            Size = size;
        }
    }
}