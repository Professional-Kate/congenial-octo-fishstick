using IdelPog.Common.DTO.Error;

namespace IdelPog.Common.Factories
{
    public interface IErrorDTOFactory
    {
        public ErrorDTO Create(Exception exception);
    }
}