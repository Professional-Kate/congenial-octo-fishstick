using IdelPog.Common.Enums;

namespace ContentEngine.Runtime.Services
{
    public interface ISkillNodeAccessValidator
    {
        public void AssertSkillAllows(SkillID skillID, ResourceID resourceID);
    }
}