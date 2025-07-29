using IdelPog.SimulationEngine.Structures.Level;

namespace IdelPog.SimulationEngine.Skill
{
    public class SkillUpdateFactory(ILevelProgressFactory levelProgressFactory) : ISkillUpdateFactory
    {
        public SkillUpdateResponse CreateSkillUpdate(Models.Skill skill, bool hasLeveled)
        {
            return new SkillUpdateResponse
            {
                SkillID = skill.SkillID,
                HasLeveled = hasLeveled,
                LevelProgress = levelProgressFactory.CreateLevelableUpdate(skill.Levelable)
            };
        }
    }
}