namespace IdelPog.Core.Validation.Exceptions
{
    public class DuplicateEntityException : Exception
    {
        private const string MESSAGE = "The passed Item {0} already exists!!";

        public readonly object ID;

        public DuplicateEntityException(object id) : base(string.Format(MESSAGE, id))
        {
            ID = id;
        }
    }
}