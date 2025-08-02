using IdelPog.Common.Commands;
using IdelPog.Common.Responses;

namespace IdelPog.Common.Factories
{
    public class SkillChangeResponseFactory : ISkillChangeResponseFactory
    {
        public SkillChangeResponse Create(SkillChange skillChange)
        {
            return new SkillChangeResponse
            {
                SkillID = skillChange.SkillID
            };
        }
    }
}