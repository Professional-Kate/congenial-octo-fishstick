namespace IdelPog.Core.Validation.Exceptions
{
    public class EmptyCollectionException : Exception
    {
        private const string MESSAGE = "The collection of type '{0}' was empty.";

        public readonly Type CollectionType;

        public EmptyCollectionException(Type collection) : base(string.Format(MESSAGE, collection))
        {
            CollectionType = collection;
        }
    }
}