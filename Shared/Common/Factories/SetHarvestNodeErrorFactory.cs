using IdelPog.Common.Commands;
using IdelPog.Common.Errors;

namespace IdelPog.Common.Factories
{
    public class SetHarvestNodeErrorFactory : IErrorFactory<SetHarvestNodeError, SetHarvestNode>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public SetHarvestNodeErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public SetHarvestNodeError Create<TException>(SetHarvestNode context, TException exception) where TException : Exception
        {
            return new SetHarvestNodeError
            {
                SetHarvestNode = context,
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}