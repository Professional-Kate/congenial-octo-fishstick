using IdelPog.Common.Repository;
using IdelPog.SimulationEngine.Flows.Skill;
using IdelPog.SimulationEngine.Models;
using IdelPog.SimulationEngine.Service;
using IdelPog.SimulationEngine.Structures.Types;

namespace IdelPog.SimulationEngine.Orchestration
{
    public class JobMediator(IExperienceService experienceService, ILevelService levelService, IStateRepository<SkillID, Skill> stateRepository)
        : IJobMediator
    {
        public ServiceResponse ProcessJobAction(SkillID skillID)
        {
            try
            {
                Skill skill = stateRepository.Get(skillID);
                ILevelable levelable = skill.Levelable;
                
                experienceService.AddExperience(levelable);

                if (levelService.CanJobLevel(levelable))
                {
                    levelService.LevelUpJob(levelable);
                }
                
                stateRepository.Update(skillID, skill);
            }
            catch (Exception exception)
            {
                return ServiceResponse.Failure(exception.Message);
            }
            
            return ServiceResponse.Success();
        }
    }
}