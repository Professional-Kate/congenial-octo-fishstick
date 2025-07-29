using IdelPog.Common.Commands;
using IdelPog.Common.Factories;
using IdelPog.Common.Responses;
using IdelPog.Messaging.Dispatch;

namespace IdelPog.SimulationEngine.Skill
{
    public class SkillChangeMediator : ISkillChangeMediator
    {
        private readonly ICurrentSkillSetter _currentSkillSetter;
        private readonly ISkillChangeResponseFactory _skillChangeResponseFactory;
        private readonly IDispatchOne<SkillChangeResponse> _skillChangeDTODispatcher;

        public SkillChangeMediator(ICurrentSkillSetter currentSkillSetter, ISkillChangeResponseFactory skillChangeResponseFactory,
            IDispatchOne<SkillChangeResponse> skillChangeDTODispatcher)
        {
            _currentSkillSetter = currentSkillSetter;
            _skillChangeResponseFactory = skillChangeResponseFactory;
            _skillChangeDTODispatcher = skillChangeDTODispatcher;
        }

        public void ChangeSkill(SkillChange skillChange)
        {
            _currentSkillSetter.SetCurrentSkill(skillChange.SkillID);
            _skillChangeDTODispatcher.Dispatch(_skillChangeResponseFactory.Create(skillChange));
        }
    }
}