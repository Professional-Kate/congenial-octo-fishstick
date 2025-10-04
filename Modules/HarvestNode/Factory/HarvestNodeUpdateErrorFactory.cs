using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Factory.Interface;

namespace IdelPog.HarvestNode.Factory
{
    public class HarvestNodeUpdateErrorFactory : IErrorFactory<HarvestNodeUpdateError, IReadOnlyList<HarvestNodeUpdate>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public HarvestNodeUpdateErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public HarvestNodeUpdateError Create<TException>(TException exception, IReadOnlyList<HarvestNodeUpdate> context) where TException : Exception
        {
            return new HarvestNodeUpdateError
            {
                HarvestNodeUpdates = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}