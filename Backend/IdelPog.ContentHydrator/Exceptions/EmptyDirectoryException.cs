using IdelPog.Validation.Constants;

namespace IdelPog.ContentHydrator.Exceptions
{
    public class EmptyDirectoryException : Exception
    {
        private static readonly string _baseMessage = ExceptionConstants.EMPTY_DIRECTORY_MESSAGE;

        public EmptyDirectoryException(string path) 
            : base(string.Format(_baseMessage, path))
        {
            // TODO: whenever will I get a logging framework
        }
    }
}