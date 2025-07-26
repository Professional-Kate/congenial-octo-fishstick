using IdelPog.Common.DTO;

namespace IdelPog.Common.Factories
{
    public interface IErrorDTOFactory
    {
        public ErrorDTO Create(Exception exception);
    }
}