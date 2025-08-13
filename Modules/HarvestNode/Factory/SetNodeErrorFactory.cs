using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Factory.Interface;

namespace IdelPog.HarvestNode.Factory
{
    public class SetNodeErrorFactory : IErrorFactory<SetHarvestNodeError, SetHarvestNode>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public SetNodeErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public SetHarvestNodeError Create<TException>(TException exception, SetHarvestNode context) where TException : Exception
        {
            return new SetHarvestNodeError
            {
                SetHarvestNode = context,
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}