namespace IdelPog.Infrastructure.Structures
{
    /// <summary>
    /// <see cref="ICloneable"/> but will return the actual object type
    /// </summary>
    /// <typeparam name="T">The type of the object to be returned</typeparam>
    /// <seealso cref="DeepClone"/>
    public interface ICloneable<out T>
    {
        /// <summary>
        /// Clone an object and return it as the type of T
        /// </summary>
        /// <returns>The cloned object of type T</returns>
        public T DeepClone();
    }
}