using IdelPog.Model;
using IdelPog.Validation.Assertions.Interfaces;
using IdelPog.Validation.Interfaces;

namespace IdelPog.Service
{
    public class ExperienceService : IExperienceService
    {
        // TODO: possible improvement. Instead of passing in 3 different assertions, maybe group them in a JobValidator(Job)? 
        private readonly IAssertUnderMaxLevel _assertUnderMaxLevel;
        private readonly IAssertNotNull _assertNotNull;
        private readonly IAssertPositive _assertPositive;

        public ExperienceService(IAssertUnderMaxLevel assertUnderMaxLevel, IAssertNotNull assertNotNull, IAssertPositive assertPositive)
        {
            _assertUnderMaxLevel = assertUnderMaxLevel;
            _assertNotNull = assertNotNull;
            _assertPositive = assertPositive;
        }
        
        public void AddExperience(Job job)
        {
            _assertNotNull.AssertObjectNotNull(job);
            _assertUnderMaxLevel.AssertLevelIsUnderMax(job);
            _assertPositive.AssertNumberIsPositive(job.ExperiencePerAction);
            
            int experienceToAdd = job.ExperiencePerAction;
            job.AddExperience(experienceToAdd);
        }
    }
}