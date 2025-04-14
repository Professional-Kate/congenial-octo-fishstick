namespace ContentHydrator.Readers
{
    /// <summary>
    /// Represents a file reader that reads flat files from a specified directory
    /// </summary>
    public interface IReader
    {
        public string BaseFilePath { get; }
        
        /// <summary>
        /// Read each file inside the <see cref="BaseFilePath"/> directory and return their raw text
        /// </summary>
        /// <returns>A collection of strings got from the file</returns>
        public IEnumerable<string> Read();
    }
}