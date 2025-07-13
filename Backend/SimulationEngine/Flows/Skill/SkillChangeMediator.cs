namespace IdelPog.SimulationEngine.Skill
{
    public class SkillChangeMediator : ISkillChangeMediator
    {
        private readonly ICurrentSkillSetter _currentSkillSetter;
        private readonly ISkillChangeFactory _skillChangeFactory;
        private readonly ISkillChangeDispatcher _skillChangeDispatcher;

        public SkillChangeMediator(ICurrentSkillSetter currentSkillSetter, ISkillChangeFactory skillChangeFactory, ISkillChangeDispatcher skillChangeDispatcher)
        {
            _currentSkillSetter = currentSkillSetter;
            _skillChangeFactory = skillChangeFactory;
            _skillChangeDispatcher = skillChangeDispatcher;
        }

        public void ChangeSkill(SkillChange skillChange)
        {
            _currentSkillSetter.SetCurrentSkill(skillChange.SkillID);
            _skillChangeDispatcher.Dispatch(_skillChangeFactory.CreateSkillChangeDTO(skillChange));
        }
    }
}