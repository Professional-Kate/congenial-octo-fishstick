using IdelPog.Common.Repository;
using IdelPog.SimulationEngine.Models;
using IdelPog.SimulationEngine.Service;
using IdelPog.SimulationEngine.Structures.Types;

namespace IdelPog.SimulationEngine.Flows.Skill
{
    public class SkillMediator : ISkillMediator
    {
        private readonly IExperienceService _experienceService;
        private readonly ILevelService _levelService;
        private readonly IStateRepository<SkillID, Skill>  _skillRepository;
        private readonly ICurrentSkillProvider _currentSkillProvider;
        
        public SkillMediator(IExperienceService experienceService, ILevelService levelService, IStateRepository<SkillID, Skill> skillRepository,  ICurrentSkillProvider currentSkillProvider)
        {
            _experienceService = experienceService;
            _levelService = levelService;
            _skillRepository = skillRepository;
            _currentSkillProvider = currentSkillProvider;
        }
        
        public ServiceResponse ProcessSkillAction()
        {
            // TODO: this method will be called by the tick controller (when I make it)
            SkillID currentSkillID = _currentSkillProvider.GetCurrentSkill();
            
            try
            {
                Skill skill = _skillRepository.Get(currentSkillID);
                ILevelable levelable = skill.Levelable;
                
                _experienceService.AddExperience(levelable);

                if (_levelService.CanSkillLevel(levelable))
                {
                    _levelService.LevelUpSkill(levelable);
                }
                
                _skillRepository.Update(currentSkillID, skill);
            }
            catch (Exception exception)
            {
                return ServiceResponse.Failure(exception.Message);
            }
            
            return ServiceResponse.Success();
        }
    }
}