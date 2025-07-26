using IdelPog.Common.Enums;

namespace IdelPog.SimulationEngine.Skill
{
    public interface ICurrentSkillSetter
    {
        public void SetCurrentSkill(SkillID skill);
    }
}