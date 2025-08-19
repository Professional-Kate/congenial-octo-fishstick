using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Messaging.Listener.Single;
using IdelPog.Core.Progression;
using IdelPog.Core.Progression.Experience;
using IdelPog.Core.Progression.Level;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Scheduler;
using IdelPog.Loot.Service.Interface;
using IdelPog.Skill.Factory.Interface;
using IdelPog.Skill.Service;

namespace IdelPog.Skill.Mediator
{
    public class SkillActionMediator : ISingleMediator<ScheduleTick>
    {
        private readonly IExperienceService _experienceService;
        private readonly ILevelService _levelService;
        private readonly IStateRepository<SkillID, Contracts.Skill> _skillRepository;
        private readonly ICurrentSkillProvider _currentSkillProvider;
        private readonly IDispatchOne<SkillUpdateResponse> _skillUpdateDTODispatcher;
        private readonly ISkillUpdateResponseFactory _skillUpdateResponseFactory;
        private readonly ILootService<SkillID> _lootService;

        public SkillActionMediator(IExperienceService experienceService, ILevelService levelService, IStateRepository<SkillID, Contracts.Skill> skillRepository,
            ICurrentSkillProvider currentSkillProvider, IDispatchOne<SkillUpdateResponse> skillUpdateDTODispatcher, ISkillUpdateResponseFactory skillUpdateResponseFactory, ILootService<SkillID> lootService)
        {
            _experienceService = experienceService;
            _levelService = levelService;
            _skillRepository = skillRepository;
            _currentSkillProvider = currentSkillProvider;
            _skillUpdateDTODispatcher = skillUpdateDTODispatcher;
            _skillUpdateResponseFactory = skillUpdateResponseFactory;
            _lootService = lootService;
        }

        public void HandleMessage(ScheduleTick message)
        {
            SkillID currentSkillID = _currentSkillProvider.GetCurrentSkill();

            Contracts.Skill skill = _skillRepository.Get(currentSkillID);
            Levelable levelable = skill.Levelable;

            _experienceService.AddExperience(levelable);
            bool canSkillLevel = _levelService.CanLevel(levelable);

            if (canSkillLevel)
            {
                _levelService.LevelUp(levelable);
            }

            _lootService.DispatchInventoryUpdates(currentSkillID);
            
            _skillRepository.Update(currentSkillID, skill);
            _skillUpdateDTODispatcher.Dispatch(_skillUpdateResponseFactory.Create(skill, canSkillLevel));
        }
    }
}