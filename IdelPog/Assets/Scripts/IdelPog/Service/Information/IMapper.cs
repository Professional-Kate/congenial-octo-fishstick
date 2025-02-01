using IdelPog.Structures;

namespace IdelPog.Service
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="GetInformation"/>
    public interface IMapper<in T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Information GetInformation(T key);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="information"></param>
        public void AddInformation(T key, Information information);
    }
}