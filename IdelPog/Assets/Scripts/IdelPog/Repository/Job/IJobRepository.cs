using IdelPog.Model;
using IdelPog.Structures.Enums;

namespace IdelPog.Repository
{
    /// <summary>
    /// Provides CRUD operations for the <see cref="Job"/> model
    /// </summary>
    /// <seealso cref="Add"/>
    /// <seealso cref="Remove"/>
    /// <seealso cref="Get"/>
    /// <seealso cref="Update"/>
    public interface IJobRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(JobType key, Job value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void Remove(JobType key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Job Get(JobType key);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Update(JobType key, Job value);
    }
}