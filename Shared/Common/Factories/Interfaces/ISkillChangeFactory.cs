using IdelPog.SimulationEngine.Skill;

namespace IdelPog.Common.Factories
{
    public interface ISkillChangeFactory
    {
        public SkillChange CreateSkillChange(SkillID skillID);
    }
}