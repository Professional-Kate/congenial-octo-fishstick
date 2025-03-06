using System;
using IdelPog.Validation.Constants;

namespace IdelPog.Validation
{
    public class NotFoundException : Exception
    {
        private static readonly string _baseMessage = ExceptionConstants.NOT_FOUND_MESSAGE;
        
        public NotFoundException(object id, Type callingClass) 
            : base(string.Format(_baseMessage, id, callingClass.Name))
        {
            // TODO: This needs to be logged to file.
        }

    }
}