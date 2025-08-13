using IdelPog.Core.Contracts.Enum;

namespace IdelPog.HarvestNode.Runtime.System.Interface
{
    public interface ISkillNodeAccessValidator
    {
        public void AssertSkillAllows(SkillID skillID, ResourceID resourceID);
    }
}