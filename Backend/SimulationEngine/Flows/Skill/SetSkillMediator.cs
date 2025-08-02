using IdelPog.Common.Commands;
using IdelPog.Common.Factories;
using IdelPog.Common.Responses;
using IdelPog.Messaging.Dispatch.Single;
using IdelPog.Messaging.Listeners.Single;

namespace IdelPog.SimulationEngine.Skill
{
    public class SetSkillMediator : ISingleMediator<SetSkill>
    {
        private readonly ICurrentSkillSetter _currentSkillSetter;
        private readonly ISetSkillResponseFactory _setSkillResponseFactory;
        private readonly IDispatchOne<SetSkillResponse> _skillChangeDTODispatcher;

        public SetSkillMediator(ICurrentSkillSetter currentSkillSetter, ISetSkillResponseFactory setSkillResponseFactory,
            IDispatchOne<SetSkillResponse> skillChangeDTODispatcher)
        {
            _currentSkillSetter = currentSkillSetter;
            _setSkillResponseFactory = setSkillResponseFactory;
            _skillChangeDTODispatcher = skillChangeDTODispatcher;
        }

        public void HandleMessage(SetSkill skillChange)
        {
            _currentSkillSetter.SetCurrentSkill(skillChange.SkillID);
            _skillChangeDTODispatcher.Dispatch(_setSkillResponseFactory.Create(skillChange));
        }
    }
}