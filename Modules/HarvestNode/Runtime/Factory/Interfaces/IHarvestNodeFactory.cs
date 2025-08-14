using IdelPog.Core.Contracts.Enum;

namespace IdelPog.HarvestNode.Runtime.Factory.Interfaces
{
    public interface IHarvestNodeFactory
    {
        public Contracts.HarvestNode Create(ItemID itemID);
    }
}