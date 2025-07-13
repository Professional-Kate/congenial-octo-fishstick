namespace IdelPog.Validation.Exceptions
{
    public class CollectionEmptyException : Exception
    {
        private const string MESSAGE = "The passed collection of type {0} is empty. This is not allowed, naughty naughty!!";

        public readonly Type CollectionType;

        public CollectionEmptyException(Type collectionType) : base(string.Format(MESSAGE, collectionType.Name))
        {
            CollectionType = collectionType;
        }
    }
}