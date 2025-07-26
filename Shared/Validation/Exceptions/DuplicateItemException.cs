using IdelPog.Validation.Constants;

namespace IdelPog.Validation.Exceptions
{
    public class DuplicateItemException : Exception
    {
        private static readonly string _baseMessage = ExceptionConstants.DUPLICATE_ITEM_MESSAGE;

        public DuplicateItemException(object item)
            : base(string.Format(_baseMessage, item))
        {
            // TODO: whenever will I get a logging framework
        }
    }
}