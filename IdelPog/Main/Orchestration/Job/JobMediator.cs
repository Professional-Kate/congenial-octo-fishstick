using IdelPogTemp.Main.Repository;
using IdelPogTemp.Main.Service.Experience;
using IdelPogTemp.Main.Service.Level;
using IdelPogTemp.Main.Structures;
using IdelPogTemp.Main.Structures.Enums;
using IdelPogTemp.Main.Structures.Models.Levelable;

namespace IdelPogTemp.Main.Orchestration.Job
{
    public class JobMediator : IJobMediator
    {
        private readonly IExperienceService _experienceService;
        private readonly ILevelService _levelService;
        private readonly IRepository<JobType, Structures.Models.Job> _repository;

        public JobMediator(IExperienceService experienceService, ILevelService levelService, IRepository<JobType, Structures.Models.Job> repository)
        {
            _experienceService = experienceService;
            _levelService = levelService;
            _repository = repository;
        }
        
        public ServiceResponse ProcessJobAction(JobType jobType)
        {
            try
            {
                Structures.Models.Job job = _repository.Get(jobType);
                ILevelable levelable = job.Levelable;
                
                _experienceService.AddExperience(levelable);

                if (_levelService.CanJobLevel(levelable))
                {
                    _levelService.LevelUpJob(levelable);
                }
                
                _repository.Update(jobType, job);
            }
            catch (Exception exception)
            {
                return ServiceResponse.Failure(exception.Message);
            }
            
            return ServiceResponse.Success();
        }
    }
}