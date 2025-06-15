using IdelPog.Infrastructure.Repository;
using IdelPog.SimulationEngine.Models;
using IdelPog.SimulationEngine.Service;
using IdelPog.SimulationEngine.Structures.Enums;
using IdelPog.SimulationEngine.Structures.Types;

namespace IdelPog.SimulationEngine.Orchestration
{
    public class JobMediator(IExperienceService experienceService, ILevelService levelService, IStateRepository<JobType, Job> stateRepository)
        : IJobMediator
    {
        public ServiceResponse ProcessJobAction(JobType jobType)
        {
            try
            {
                Job job = stateRepository.Get(jobType);
                ILevelable levelable = job.Levelable;
                
                experienceService.AddExperience(levelable);

                if (levelService.CanJobLevel(levelable))
                {
                    levelService.LevelUpJob(levelable);
                }
                
                stateRepository.Update(jobType, job);
            }
            catch (Exception exception)
            {
                return ServiceResponse.Failure(exception.Message);
            }
            
            return ServiceResponse.Success();
        }
    }
}