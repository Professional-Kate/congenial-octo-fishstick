using System;

namespace IdelPog.Exceptions
{
    public class MaxLevelException : Exception
    {
        public MaxLevelException()
        {
            // TODO: LOGGO FILEO
        }
        
        public MaxLevelException(string message) : base(message)
        {
            // TODO: This needs to be logged to file.
        }
    }
}