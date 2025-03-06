using System;

namespace IdelPog.Validation.Exceptions
{
    public class NegativeNumberException : Exception
    {
        public NegativeNumberException(string message) : base(message)
        {
            // TODO: LOG TO FILE
        }
    }
}