using IdelPog.Core.Contracts.Enum;
using IdelPog.HarvestNode.Runtime.ECS;
using IdelPog.HarvestNode.Runtime.Factory.Interfaces;

namespace IdelPog.HarvestNode.Runtime.Factory
{
    public class SkillNodeEntityFactory : ISkillNodeEntityFactory
    {
        public SkillNodeEntity Create(SkillID skillID, ItemID[] itemIDs)
        {
            HarvestTargetComponent[] resourceComponents = new HarvestTargetComponent[itemIDs.Length];
            for (int i = 0; i < itemIDs.Length; i++)
            {
                resourceComponents[i] = new HarvestTargetComponent { HarvestTarget = itemIDs[i] };
            }
            
            SkillComponent skillComponent = new() { SkillID = skillID };
            return new SkillNodeEntity(skillComponent, resourceComponents);
        }
    }
}