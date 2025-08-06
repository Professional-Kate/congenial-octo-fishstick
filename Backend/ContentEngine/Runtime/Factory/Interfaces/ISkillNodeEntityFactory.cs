using ContentEngine.Runtime.ECS;
using IdelPog.Common.Enums;

namespace ContentEngine.Runtime.Factory.Interfaces
{
    public interface ISkillNodeEntityFactory
    {
        public SkillNodeEntity Create(SkillID skillID, ResourceID[] resourceIDs);
    }
}