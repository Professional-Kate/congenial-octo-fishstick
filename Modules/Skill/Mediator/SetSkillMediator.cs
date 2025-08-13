using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Messaging.Listener.Single;
using IdelPog.Skill.Factory.Interface;
using IdelPog.Skill.Service;

namespace IdelPog.Skill.Mediator
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