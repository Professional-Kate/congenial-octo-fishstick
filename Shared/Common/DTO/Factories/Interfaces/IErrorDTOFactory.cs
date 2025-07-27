namespace IdelPog.Common.DTO.Factories
{
    public interface IErrorDTOFactory
    {
        public ErrorDTO Create(Exception exception);
    }
}