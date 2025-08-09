using IdelPog.Common.Responses;

namespace IdelPog.SimulationEngine.Skill
{
    public interface ISkillUpdateResponseFactory
    {
        public SkillUpdateResponse Create(Models.Skill skill, bool hasLeveled);
    }
}