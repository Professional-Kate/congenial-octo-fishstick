using IdelPog.Core.Contracts;

namespace IdelPog.HarvestNode.Runtime.Factory.Interface
{
    public interface IHarvestNodeFactory
    {
        public Contracts.HarvestNode Create(ReadOnlyHarvestNode readOnlyHarvestNode);
    }
}