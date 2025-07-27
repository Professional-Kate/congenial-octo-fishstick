namespace IdelPog.Common.DTO.Factories
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