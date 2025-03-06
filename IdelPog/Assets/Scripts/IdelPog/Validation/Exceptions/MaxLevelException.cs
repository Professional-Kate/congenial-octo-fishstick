using System;
using IdelPog.Validation.Constants;

namespace IdelPog.Validation.Exceptions
{
    public class MaxLevelException : Exception
    {
        private static readonly string _baseMessage = ExceptionConstants.MAX_LEVEL_MESSAGE;
        
        public MaxLevelException(object id, Type callingClass) 
            : base(string.Format(_baseMessage, id, callingClass.Name))
        {
            // TODO: This needs to be logged to file.
        }
    }
}