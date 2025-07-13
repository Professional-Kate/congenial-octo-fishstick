using IdelPog.SimulationEngine.Models;

namespace IdelPog.SimulationEngine.Skill
{
    public class SkillUpdateFactory(ILevelableUpdateFactory levelableUpdateFactory) : ISkillUpdateFactory
    {
        public SkillUpdateDTO CreateSkillUpdate(Skill skill, bool hasLeveled)
        {
            return new SkillUpdateDTO
            {
                SkillID = skill.SkillID,
                HasLeveled = hasLeveled,
                LevelableUpdateDTO = levelableUpdateFactory.CreateLevelableUpdate(skill.Levelable)
            };
        }
    }
}