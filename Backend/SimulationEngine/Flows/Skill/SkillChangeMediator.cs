using IdelPog.Common.Commands;
using IdelPog.Messaging.Dispatch;

namespace IdelPog.SimulationEngine.Skill
{
    public class SkillChangeMediator : ISkillChangeMediator
    {
        private readonly ICurrentSkillSetter _currentSkillSetter;
        private readonly ISkillChangeFactory _skillChangeFactory;
        private readonly IDispatchOne<SkillChangeDTO> _skillChangeDTODispatcher;

        public SkillChangeMediator(ICurrentSkillSetter currentSkillSetter, ISkillChangeFactory skillChangeFactory, IDispatchOne<SkillChangeDTO> skillChangeDTODispatcher)
        {
            _currentSkillSetter = currentSkillSetter;
            _skillChangeFactory = skillChangeFactory;
            _skillChangeDTODispatcher = skillChangeDTODispatcher;
        }

        public void ChangeSkill(SkillChange skillChange)
        {
            _currentSkillSetter.SetCurrentSkill(skillChange.SkillID);
            _skillChangeDTODispatcher.Dispatch(_skillChangeFactory.CreateSkillChangeDTO(skillChange));
        }
    }
}