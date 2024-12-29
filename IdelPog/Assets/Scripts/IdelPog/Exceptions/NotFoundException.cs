using System;

namespace IdelPog.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException()
        {
            // TODO: This needs to be logged to file.
        }
        
        public NotFoundException(string message) : base(message)
        {
            // TODO: This needs to be logged to file.
        }
    }
}