using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;
using IdelPog.HarvestNode.Runtime.ECS;

namespace IdelPog.HarvestNode.Runtime.Factory.Interface
{
    public interface ISkillNodeEntityFactory
    {
        public SkillNodeEntity Create(SkillID skillID, ReadOnlyHarvestNode[] readOnlyHarvestNodes);
    }
}