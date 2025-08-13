using IdelPog.Core.Contracts.Error;

namespace IdelPog.Core.Factory.Interface
{
    public interface IBaseErrorFactory
    {
        public BaseError Create(Exception exception);
    }
}