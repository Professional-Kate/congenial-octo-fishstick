using IdelPog.SimulationEngine.Structures;

namespace IdelPog.SimulationEngine.Skill
{
    public class SkillUpdateFactory(ILevelableUpdateFactory levelableUpdateFactory) : ISkillUpdateFactory
    {
        public SkillUpdateDTO CreateSkillUpdate(Models.Skill skill, bool hasLeveled)
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