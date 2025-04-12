using IdelPog.Engine.Repository;
using IdelPog.Engine.Service.Experience;
using IdelPog.Engine.Service.Level;
using IdelPog.Engine.Structures.Enums;
using IdelPog.Engine.Structures.Levelable;
using IdelPog.Engine.Structures.Types;

namespace IdelPog.Engine.Orchestration.Job
{
    public class JobMediator(IExperienceService experienceService, ILevelService levelService, IRepository<JobType, Structures.Job> repository)
        : IJobMediator
    {
        public ServiceResponse ProcessJobAction(JobType jobType)
        {
            try
            {
                Structures.Job job = repository.Get(jobType);
                ILevelable levelable = job.Levelable;
                
                experienceService.AddExperience(levelable);

                if (levelService.CanJobLevel(levelable))
                {
                    levelService.LevelUpJob(levelable);
                }
                
                repository.Update(jobType, job);
            }
            catch (Exception exception)
            {
                return ServiceResponse.Failure(exception.Message);
            }
            
            return ServiceResponse.Success();
        }
    }
}