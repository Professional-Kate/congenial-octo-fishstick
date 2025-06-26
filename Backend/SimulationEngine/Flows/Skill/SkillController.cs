namespace IdelPog.SimulationEngine.Flows.Skill
{
    public class SkillController(ICurrentSkillSetter currentSkillSetter) : ISkillController
    {
        public void SwitchSkill(SkillChange skillChange)
        {
            currentSkillSetter.SetCurrentSkill(skillChange.SkillID);
        }
    }
}