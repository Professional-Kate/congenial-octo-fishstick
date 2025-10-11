using IdelPog.Core.Contracts;

namespace IdelPog.Core.Factory.Interface
{
    public interface IBaseErrorFactory
    {
        public BaseError Create(Exception exception);
    }
}