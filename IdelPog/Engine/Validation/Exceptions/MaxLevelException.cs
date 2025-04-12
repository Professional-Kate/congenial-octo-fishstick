using IdelPog.Engine.Validation.Constants;

namespace IdelPog.Engine.Validation.Exceptions
{
    public class MaxLevelException : Exception
    {
        private static readonly string _baseMessage = ExceptionConstants.MAX_LEVEL_MESSAGE;
        
        public MaxLevelException(object id) 
            : base(string.Format(_baseMessage, id))
        {
            // TODO: This needs to be logged to file.
        }
    }
}