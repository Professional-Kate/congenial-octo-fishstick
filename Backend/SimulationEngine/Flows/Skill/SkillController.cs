using IdelPog.Common.Commands;

namespace IdelPog.SimulationEngine.Skill
{
    public class SkillController : ISkillController
    {
        private readonly ISkillChangeMediator _skillChangeMediator;

        public SkillController(ISkillChangeMediator skillChangeMediator)
        {
            _skillChangeMediator = skillChangeMediator;
        }

        public void ChangeSkill(SetSkill setSkill)
        {
            _skillChangeMediator.ChangeSkill(setSkill);
        }
    }
}