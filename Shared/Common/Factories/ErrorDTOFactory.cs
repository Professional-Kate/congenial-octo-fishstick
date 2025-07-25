using IdelPog.Common.DTO;

namespace IdelPog.Common.Factories
{
    public class ErrorDTOFactory : IErrorDTOFactory
    {
        public ErrorDTO Create(Exception exception)
        {
            return new ErrorDTO
            {
                Exception = exception
            };
        }
    }
}