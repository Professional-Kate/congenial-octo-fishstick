using IdelPog.Common.Errors;

namespace IdelPog.Common.Factories
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