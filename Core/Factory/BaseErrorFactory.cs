using IdelPog.Core.Contracts;
using IdelPog.Core.Factory.Interface;

namespace IdelPog.Core.Factory
{
    public sealed class BaseErrorFactory : IBaseErrorFactory
    {
        public BaseError Create(Exception exception)
        {
            return new BaseError
            {
                Exception = exception
            };
        }
    }
}