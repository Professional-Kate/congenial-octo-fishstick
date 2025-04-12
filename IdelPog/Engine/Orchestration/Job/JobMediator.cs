using IdelPog.Engine.Repository;
using IdelPog.Engine.Service;
using IdelPog.Engine.Structures;
using IdelPog.Engine.Structures.Enums;
using IdelPog.Engine.Structures.Types;

namespace IdelPog.Engine.Orchestration
{
    public class JobMediator(IExperienceService experienceService, ILevelService levelService, IRepository<JobType, Job> repository)
        : IJobMediator
    {
        public ServiceResponse ProcessJobAction(JobType jobType)
        {
            try
            {
                Job job = repository.Get(jobType);
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