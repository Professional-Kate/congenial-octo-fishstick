using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Messaging.Listener.Single;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Skill.Factory.Interface;
using IdelPog.Skill.Service.Interface;

namespace IdelPog.Skill.Mediator
{
    public class SetSkillMediator : ISingleMediator<SetSkill>
    {
        private readonly ICurrentSkillSetter _currentSkillSetter;
        private readonly IStateRepository<SkillID, Contracts.Skill> _skillRepository;
        private readonly ISetSkillResponseFactory _setSkillResponseFactory;
        private readonly IDispatchOne<SetSkillResponse> _skillChangeDTODispatcher;
        private readonly IFoundAssertion _foundAssertion;

        public SetSkillMediator(ICurrentSkillSetter currentSkillSetter, IStateRepository<SkillID, Contracts.Skill> skillRepository, ISetSkillResponseFactory setSkillResponseFactory,
            IDispatchOne<SetSkillResponse> skillChangeDTODispatcher, IFoundAssertion foundAssertion)
        {
            _currentSkillSetter = currentSkillSetter;
            _skillRepository = skillRepository;
            _setSkillResponseFactory = setSkillResponseFactory;
            _skillChangeDTODispatcher = skillChangeDTODispatcher;
            _foundAssertion = foundAssertion;
        }

        public void HandleMessage(SetSkill skillChange)
        {
            _foundAssertion.AssertFound(skillChange.SkillID, _skillRepository.Contains(skillChange.SkillID));
            
            Contracts.Skill skill = _skillRepository.Get(skillChange.SkillID);
            _currentSkillSetter.SetCurrentSkill(skillChange.SkillID);
            
            _skillChangeDTODispatcher.Dispatch(_setSkillResponseFactory.Create(skill));
        }
    }
}