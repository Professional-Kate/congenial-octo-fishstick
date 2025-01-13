using System;

namespace IdelPog.Exceptions
{
    public class NoTypeException : Exception
    {
        public NoTypeException(string message) : base(message)
        {
            // TODO: This needs to be logged to file.
        }
    }
}