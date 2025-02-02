using System;
using IdelPog.Constants;

namespace IdelPog.Exceptions
{
    public class BadNumberException : Exception
    {
        public override string Message => ErrorConstants.BAD_NUMBER_MESSAGE;

        public BadNumberException()
        {
            // TOOD: LOG TO FILE
        }
    }
}