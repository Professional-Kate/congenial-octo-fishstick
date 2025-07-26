namespace IdelPog.Validation.Exceptions
{
    public class NotFoundException<TKey> : Exception
    {
        private const string MESSAGE = "Error! The passed ID {0} was not found!";

        public readonly TKey Key;

        public NotFoundException(TKey key) : base(string.Format(MESSAGE, key))
        {
            Key = key;
        }
    }
}