namespace IdelPog.SimulationEngine.Skill
{
    public class SkillController : ISkillController
    {
        private ISkillChangeMediator _skillChangeMediator;
        
        public SkillController(ISkillChangeMediator skillChangeMediator)
        {
            _skillChangeMediator = skillChangeMediator;
        }
        
        public void SwitchSkill(SkillChange skillChange)
        {
            _skillChangeMediator.SwitchSkill(skillChange);
        }
    }
}