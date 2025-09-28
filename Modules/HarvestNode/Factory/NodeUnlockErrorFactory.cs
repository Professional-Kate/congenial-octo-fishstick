using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Factory.Interface;

namespace IdelPog.HarvestNode.Factory
{
    public sealed class NodeUnlockErrorFactory : IErrorFactory<HarvestNodeUnlockError, IReadOnlyList<HarvestNodeUnlock>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public NodeUnlockErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public HarvestNodeUnlockError Create<TException>(TException exception, IReadOnlyList<HarvestNodeUnlock> context) where TException : Exception
        {
            return new HarvestNodeUnlockError
            {
                HarvestNodeUnlocks = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}