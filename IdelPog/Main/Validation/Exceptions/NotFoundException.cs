using IdelPogTemp.Main.Validation.Constants;

namespace IdelPogTemp.Main.Validation.Exceptions
{
    public class NotFoundException : Exception
    {
        private static readonly string _baseMessage = ExceptionConstants.NOT_FOUND_MESSAGE;
        
        public NotFoundException(object id) 
            : base(string.Format(_baseMessage, id))
        {
            // TODO: This needs to be logged to file.
        }

    }
}