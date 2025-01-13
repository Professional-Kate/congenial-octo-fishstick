using IdelPog.Exceptions;
using IdelPog.Model;
using IdelPog.Structures.Enums;

namespace IdelPog.Repository
{
    /// <seealso cref="IJobRepository"/>
    public class JobRepository : Repository<JobType, Job>, IJobRepository
    {
        /// <inheritdoc cref="Add"/>
        public override void Add(JobType key, Job value)
        {
            AssertTypeIsValid(key);
            
            base.Add(key, value);
        }
        
        /// <inheritdoc cref="Remove"/>
        public override void Remove(JobType key)
        {
            AssertTypeIsValid(key);
            
            base.Remove(key);
        }
        
        /// <inheritdoc cref="Get"/>
        public override Job Get(JobType key)
        {
            AssertTypeIsValid(key);
                
            Job jobClone = base.Get(key);
            
            return jobClone.Clone() as Job;
        }
        
        /// <inheritdoc cref="Update"/>
        public override void Update(JobType key, Job value)
        {
            AssertTypeIsValid(key);
            
            base.Update(key, value);
        }
        
        /// <summary>
        /// Asserts that the passed <see cref="CurrencyType"/> is not <see cref="CurrencyType.NO_TYPE"/>
        /// </summary>
        /// <param name="jobType">The <see cref="CurrencyType"/> you want to check</param>
        /// <exception cref="NoTypeException">Will be thrown if the passed <see cref="CurrencyType"/> is <see cref="CurrencyType.NO_TYPE"/></exception>
        /// <remarks>
        /// This exception is expected to not be caught. A <see cref="Currency"/> having the type <see cref="CurrencyType.NO_TYPE"/> is a data setup issue
        /// </remarks>
        private static void AssertTypeIsValid(JobType jobType)
        {
            if (jobType == JobType.NO_TYPE)
            {
                throw new NoTypeException("Error! Passed CurrencyType is NO_TYPE, nothing can be retrieved. This should be fixed.");
            }
        }
    }
}