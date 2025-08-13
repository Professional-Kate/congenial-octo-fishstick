using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Factory.Interface;

namespace IdelPog.Core.Factory
{
    public class BaseErrorFactory : IBaseErrorFactory
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