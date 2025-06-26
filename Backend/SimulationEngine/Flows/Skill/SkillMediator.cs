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
        
        public SkillMediator(IExperienceService experienceService, ILevelService levelService, IStateRepository<SkillID, Skill> skillRepository)
        {
            _experienceService = experienceService;
            _levelService = levelService;
            _skillRepository = skillRepository;
        }
        
        public ServiceResponse ProcessSkillAction(SkillID skillID)
        {
            try
            {
                Skill skill = _skillRepository.Get(skillID);
                ILevelable levelable = skill.Levelable;
                
                _experienceService.AddExperience(levelable);

                if (_levelService.CanJobLevel(levelable))
                {
                    _levelService.LevelUpJob(levelable);
                }
                
                _skillRepository.Update(skillID, skill);
            }
            catch (Exception exception)
            {
                return ServiceResponse.Failure(exception.Message);
            }
            
            return ServiceResponse.Success();
        }
    }
}