using IdelPog.Common.Repository;
using IdelPog.SimulationEngine.Models;
using IdelPog.SimulationEngine.Service;

namespace IdelPog.SimulationEngine.Skill
{
    public class SkillActionMediator : ISkillActionMediator
    {
        private readonly IExperienceService _experienceService;
        private readonly ILevelService _levelService;
        private readonly IStateRepository<SkillID, Skill>  _skillRepository;
        private readonly ICurrentSkillProvider _currentSkillProvider;
        private readonly ISkillUpdateDispatcher _skillUpdateDispatcher;
        private readonly ISkillUpdateFactory _skillUpdateFactory;
        
        public SkillActionMediator(IExperienceService experienceService, ILevelService levelService, IStateRepository<SkillID, Skill> skillRepository,  ICurrentSkillProvider currentSkillProvider, ISkillUpdateDispatcher skillUpdateDispatcher, ISkillUpdateFactory skillUpdateFactory)
        {
            _experienceService = experienceService;
            _levelService = levelService;
            _skillRepository = skillRepository;
            _currentSkillProvider = currentSkillProvider;
            _skillUpdateDispatcher = skillUpdateDispatcher;
            _skillUpdateFactory = skillUpdateFactory;
        }
        
        public void ProcessSkillAction()
        {
            // TODO: this method will be called by the tick controller (when I make it)
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
            _skillUpdateDispatcher.Dispatch(_skillUpdateFactory.CreateSkillUpdate(skill,  canSkillLevel));
        }
    }
}