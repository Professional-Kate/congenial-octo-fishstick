namespace ContentHydrator.Service
{
    /// <summary>
    /// Converts all files in a given directory to a specified DTO
    /// </summary>
    /// <typeparam name="T">The type of the DTO</typeparam>
    public interface IDirectoryConverter
    {
        /// <summary>
        /// Converts every file in the given path to a specified DTO <typeparamref name="T"/>
        /// </summary>
        /// <param name="directoryPath">The path of the directory</param>
        /// <returns>A collection of all the converted DTOs</returns>
        public IEnumerable<T> ConvertDirectory<T>(string directoryPath);
    }
}