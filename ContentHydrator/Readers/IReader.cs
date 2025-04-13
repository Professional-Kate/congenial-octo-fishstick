namespace ContentHydrator.Readers
{
    /// <summary>
    /// Represents a file reader that reads flat files and converts them into their matching DTOs 
    /// </summary>
    /// <typeparam name="T">The DTO you wish to create from the flat files</typeparam>
    public interface IReader<out T>
    {
        public string BaseFilePath { get; }
        
        /// <summary>
        /// Read each file inside the <see cref="BaseFilePath"/> directory and converts them into DTOs 
        /// </summary>
        /// <returns>A collection of all DTOs created from the files</returns>
        public IEnumerable<T> Read();
    }
}