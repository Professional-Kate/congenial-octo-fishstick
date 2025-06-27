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
        private readonly ISkillUpdateDispatcher _skillUpdateDispatcher;
        
        public SkillMediator(IExperienceService experienceService, ILevelService levelService, IStateRepository<SkillID, Skill> skillRepository,  ICurrentSkillProvider currentSkillProvider, ISkillUpdateDispatcher skillUpdateDispatcher)
        {
            _experienceService = experienceService;
            _levelService = levelService;
            _skillRepository = skillRepository;
            _currentSkillProvider = currentSkillProvider;
            _skillUpdateDispatcher = skillUpdateDispatcher;
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
                bool canSkillLevel = _levelService.CanSkillLevel(levelable);

                if (canSkillLevel)
                {
                    _levelService.LevelUpSkill(levelable);
                }
                
                _skillRepository.Update(currentSkillID, skill);
                
                // TODO : create factory to do this
                SkillUpdateDTO skillUpdateDTO = new()
                {
                    SkillID = skill.SkillID,
                    HasLeveled = canSkillLevel,
                    LevelableUpdateDTO = new LevelableUpdateDTO
                    {
                        Experience = levelable.Experience,
                        ExperiencePerAction = levelable.ExperiencePerAction,
                        Level = levelable.Level,
                        NextLevelExperience = levelable.NextLevelExperience,
                    }
                };
                
                _skillUpdateDispatcher.Dispatch(skillUpdateDTO);
            }
            catch (Exception exception)
            {
                return ServiceResponse.Failure(exception.Message);
            }
            
            return ServiceResponse.Success();
        }
    }
}