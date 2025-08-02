using IdelPog.Common.Errors;

namespace IdelPog.Common.Factories
{
    public interface IBaseErrorFactory
    {
        public BaseError Create(Exception exception);
    }
}