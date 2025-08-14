using IdelPog.Core.Contracts.Enum;
using IdelPog.HarvestNode.Runtime.ECS;

namespace IdelPog.HarvestNode.Runtime.Factory.Interfaces
{
    public interface ISkillNodeEntityFactory
    {
        public SkillNodeEntity Create(SkillID skillID, ItemID[] itemIDs);
    }
}