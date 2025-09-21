using IdelPog.Core.Contracts;

namespace IdelPog.HarvestNode.Runtime.Factory.Interfaces
{
    public interface IHarvestNodeFactory
    {
        public Contracts.HarvestNode Create(ReadOnlyHarvestNode readOnlyHarvestNode);
    }
}