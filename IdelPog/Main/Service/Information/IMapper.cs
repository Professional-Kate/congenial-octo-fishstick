using IdelPogTemp.Main.Structures;
using IdelPogTemp.Main.Validation.Exceptions;

namespace IdelPogTemp.Main.Service.Information
{
    /// <summary>
    /// Information mapper, maps a passed key onto an <see cref="Information"/> object
    /// </summary>
    /// <seealso cref="GetInformation"/>
    /// <seealso cref="AddInformation"/>
    public interface IMapper<in T>
    {
        /// <summary>
        /// Uses a passed key to return an <see cref="Information"/> object
        /// </summary>
        /// <param name="key">The <see cref="Information"/> object you want will have this key</param>
        /// <returns>The found <see cref="Information"/> object</returns>
        /// <exception cref="NotFoundException">Will be thrown if the passed key is not found in the Dictionary</exception>
        public Structures.Information GetInformation(T key);
        
        /// <summary>
        /// Adds a key value pair into the Dictionary
        /// </summary>
        /// <param name="key">This key will be linked to the <see cref="Information"/> object</param>
        /// <param name="information">This <see cref="Information"/> object will be linked to the passed key in the Dictionary</param>
        /// <exception cref="ArgumentException">Will be thrown if the passed key already exists in the Dictionary</exception>
        public void AddInformation(T key, Structures.Information information);
    }
}