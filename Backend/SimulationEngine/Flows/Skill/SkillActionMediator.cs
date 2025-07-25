using IdelPog.Common.Enums;
using IdelPog.Common.Repository;
using IdelPog.Common.Structures;
using IdelPog.Messaging.Dispatch;
using IdelPog.SimulationEngine.Models;
using IdelPog.SimulationEngine.Service;

namespace IdelPog.SimulationEngine.Skill
{
    public class SkillActionMediator : IScheduledTask
    {
        private readonly IExperienceService _experienceService;
        private readonly ILevelService _levelService;
        private readonly IStateRepository<SkillID, Skill>  _skillRepository;
        private readonly ICurrentSkillProvider _currentSkillProvider;
        private readonly IDispatchOne<SkillUpdateDTO> _skillUpdateDTODispatcher;
        private readonly ISkillUpdateFactory _skillUpdateFactory;
        
        public SkillActionMediator(IExperienceService experienceService, ILevelService levelService, IStateRepository<SkillID, Skill> skillRepository,  ICurrentSkillProvider currentSkillProvider, IDispatchOne<SkillUpdateDTO> skillUpdateDTODispatcher, ISkillUpdateFactory skillUpdateFactory)
        {
            _experienceService = experienceService;
            _levelService = levelService;
            _skillRepository = skillRepository;
            _currentSkillProvider = currentSkillProvider;
            _skillUpdateDTODispatcher = skillUpdateDTODispatcher;
            _skillUpdateFactory = skillUpdateFactory;
        }

        public void Run()
        {
            SkillID currentSkillID = _currentSkillProvider.GetCurrentSkill();
            
            Skill skill = _skillRepository.Get(currentSkillID);
            ILevelable levelable = skill.Levelable;
            
            _experienceService.AddExperience(levelable);
            bool canSkillLevel = _levelService.CanSkillLevel(levelable);

            if (canSkillLevel)
            {
                _levelService.LevelUpSkill(levelable);
            }
            
            _skillRepository.Update(currentSkillID, skill);
            _skillUpdateDTODispatcher.Dispatch(_skillUpdateFactory.CreateSkillUpdate(skill,  canSkillLevel));
        }
    }
}