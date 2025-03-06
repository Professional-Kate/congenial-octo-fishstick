using System;
using IdelPog.Validation.Constants;

namespace IdelPog.Validation
{
    public class NegativeNumberException : Exception
    {
        private static readonly string _baseMessage = ExceptionConstants.NEGATIVE_NUMBER_EXCEPTION; 
        
        public NegativeNumberException(int number) 
            : base(string.Format(_baseMessage, number))
        {
            // TODO: LOG TO FILE
        }
    }
}