using System;
using IdelPog.Constants;

namespace IdelPog.Exceptions
{
    public class NoTypeException : Exception
    {
        public override string Message => ErrorConstants.NO_TYPE_MESSAGE;

        public NoTypeException()
        {
            // TODO: This needs to be logged to file.
        }
    }
}