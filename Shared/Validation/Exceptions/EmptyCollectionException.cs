using System.Collections;

namespace IdelPog.Validation.Exceptions
{
    public class EmptyCollectionException : Exception
    {
        private const string MESSAGE = "The collection of type '{0}' was empty. ";
        
        public readonly Type CollectionType;

        public EmptyCollectionException(IEnumerable collection) : base(string.Format(MESSAGE, collection.GetType().Name))
        {
            CollectionType = collection.GetType();
        }
    }
}