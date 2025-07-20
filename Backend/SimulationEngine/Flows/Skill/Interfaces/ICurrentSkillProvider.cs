using IdelPog.Common.Enums;

namespace IdelPog.SimulationEngine.Skill
{
    public interface ICurrentSkillProvider
    {
        public SkillID GetCurrentSkill();
    }
}