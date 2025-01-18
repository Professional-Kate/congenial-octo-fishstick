using System;

namespace IdelPog.Exceptions
{
    public class NoTypeException : Exception
    {
        public NoTypeException()
        {
            // TODO: This needs to be logged to file.
        }
        
        public NoTypeException(string message) : base(message)
        {
            // TODO: This needs to be logged to file.
        }
    }
}