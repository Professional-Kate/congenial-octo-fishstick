using IdelPog.Core.Contracts.Enum;
using IdelPog.HarvestNode.Runtime.ECS;
using IdelPog.HarvestNode.Runtime.Factory.Interfaces;

namespace IdelPog.HarvestNode.Runtime.Factory
{
    public class SkillNodeEntityFactory : ISkillNodeEntityFactory
    {
        public SkillNodeEntity Create(SkillID skillID, ResourceID[] resourceIDs)
        {
            ResourceComponent[] resourceComponents = new ResourceComponent[resourceIDs.Length];
            for (int i = 0; i < resourceIDs.Length; i++)
            {
                resourceComponents[i] = new ResourceComponent { ResourceID = resourceIDs[i] };
            }
            
            SkillComponent skillComponent = new() { SkillID = skillID };
            return new SkillNodeEntity(skillComponent, resourceComponents);
        }
    }
}