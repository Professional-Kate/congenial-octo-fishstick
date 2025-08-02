using IdelPog.Common.Enums;
using IdelPog.Common.Level;
using IdelPog.Common.Repository;
using IdelPog.Common.Responses;
using IdelPog.Common.Structures;
using IdelPog.Messaging.Dispatch.Single;
using IdelPog.SimulationEngine.Service;

namespace IdelPog.SimulationEngine.Skill
{
    public class SkillActionMediator : IScheduledTask
    {
        private readonly IExperienceService _experienceService;
        private readonly ILevelService _levelService;
        private readonly IStateRepository<SkillID, Models.Skill> _skillRepository;
        private readonly ICurrentSkillProvider _currentSkillProvider;
        private readonly IDispatchOne<SkillUpdateResponse> _skillUpdateDTODispatcher;
        private readonly ISkillUpdateResponseFactory _skillUpdateResponseFactory;

        public SkillActionMediator(IExperienceService experienceService, ILevelService levelService, IStateRepository<SkillID, Models.Skill> skillRepository,
            ICurrentSkillProvider currentSkillProvider, IDispatchOne<SkillUpdateResponse> skillUpdateDTODispatcher, ISkillUpdateResponseFactory skillUpdateResponseFactory)
        {
            _experienceService = experienceService;
            _levelService = levelService;
            _skillRepository = skillRepository;
            _currentSkillProvider = currentSkillProvider;
            _skillUpdateDTODispatcher = skillUpdateDTODispatcher;
            _skillUpdateResponseFactory = skillUpdateResponseFactory;
        }

        public void Run()
        {
            SkillID currentSkillID = _currentSkillProvider.GetCurrentSkill();

            Models.Skill skill = _skillRepository.Get(currentSkillID);
            Levelable levelable = skill.Levelable;

            _experienceService.AddExperience(levelable);
            bool canSkillLevel = _levelService.CanLevel(levelable);

            if (canSkillLevel)
            {
                _levelService.LevelUp(levelable);
            }

            _skillRepository.Update(currentSkillID, skill);
            _skillUpdateDTODispatcher.Dispatch(_skillUpdateResponseFactory.Create(skill, canSkillLevel));
        }
    }
}