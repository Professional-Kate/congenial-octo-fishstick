using IdelPog.Common.DTO.Error;

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