using IdelPog.Core.Factory.Interface;
using IdelPog.HarvestNode.Contracts.Command;
using IdelPog.HarvestNode.Contracts.Error;

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