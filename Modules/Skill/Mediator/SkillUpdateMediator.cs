using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Progression;
using IdelPog.Core.Progression.Experience;
using IdelPog.Core.Progression.Level;
using IdelPog.Core.Repository.State;
using IdelPog.Skill.Contracts.Command;
using IdelPog.Skill.Contracts.Response;
using IdelPog.Skill.Factory.Interface;

namespace IdelPog.Skill.Mediator
{
    public class SkillUpdateMediator : IBatchMediator<SkillUpdate>
    {
        private readonly IExperienceService _experienceService;
        private readonly ILevelService _levelService;
        private readonly IStateRepository<SkillID, Contracts.Skill> _skillRepository;
        private readonly IDispatchMany<SkillUpdateResponse> _skillUpdateDispatcher;
        private readonly ISkillUpdateResponseFactory _skillUpdateResponseFactory;

        public SkillUpdateMediator(IExperienceService experienceService, ILevelService levelService, IStateRepository<SkillID, Contracts.Skill> skillRepository
            , IDispatchMany<SkillUpdateResponse> skillUpdateDispatcher, ISkillUpdateResponseFactory skillUpdateResponseFactory)
        {
            _experienceService = experienceService;
            _levelService = levelService;
            _skillRepository = skillRepository;
            _skillUpdateDispatcher = skillUpdateDispatcher;
            _skillUpdateResponseFactory = skillUpdateResponseFactory;
        }

        public void HandleMessages(IReadOnlyList<SkillUpdate> messages)
        {
            SkillUpdateResponse[] responses = new SkillUpdateResponse[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                SkillUpdate skillUpdate = messages[i];
                SkillID skillID = skillUpdate.SkillID;

                Contracts.Skill skill = _skillRepository.Get(skillID);
                Levelable levelable = skill.Levelable;

                _experienceService.AddExperience(levelable);
                bool canSkillLevel = _levelService.CanLevel(levelable);

                if (canSkillLevel)
                {
                    _levelService.LevelUp(levelable);
                }

                // _lootService.GenerateItemID(skillID);

                _skillRepository.Update(skillID, skill);
                responses[i] = _skillUpdateResponseFactory.Create(skill, canSkillLevel);
            }
            
            _skillUpdateDispatcher.Dispatch(responses);
        }
    }
}