using IdelPog.Main.Validation.Constants;

namespace IdelPog.Main.Validation.Exceptions
{
    public class NegativeNumberException : Exception
    {
        private static readonly string _baseMessage = ExceptionConstants.NEGATIVE_NUMBER_MESSAGE; 
        
        public NegativeNumberException(int number) 
            : base(string.Format(_baseMessage, number))
        {
            // TODO: LOG TO FILE
        }
    }
}