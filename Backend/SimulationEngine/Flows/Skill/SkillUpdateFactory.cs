using IdelPog.SimulationEngine.Models;

namespace IdelPog.SimulationEngine.Flows.Skill
{
    public class SkillUpdateFactory(ILevelableUpdateFactory levelableUpdateFactory) : ISkillUpdateFactory
    {
        public SkillUpdateDTO CreateSkillUpdate(Skill skill, bool canSkillLevel)
        {
            return new SkillUpdateDTO
            {
                SkillID = skill.SkillID,
                HasLeveled = canSkillLevel,
                LevelableUpdateDTO = levelableUpdateFactory.CreateLevelableUpdate(skill.Levelable)
            };
        }
    }
}