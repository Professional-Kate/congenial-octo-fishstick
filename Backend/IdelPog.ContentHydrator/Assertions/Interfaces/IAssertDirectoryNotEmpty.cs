namespace IdelPog.ContentHydrator.Assertions
{
    /// <summary>
    /// Asserts that a directory, which is represented only by its content, is not empty. This should be used before trying to lead content from flat files 
    /// </summary>
    public interface IAssertDirectoryNotEmpty
    {
        /// <summary>
        /// Asserts that the passed items, which should represent the content of a directory, is not empty.
        /// </summary>
        /// <param name="items">The array of file names that represent the content of a directory</param>
        /// <param name="directoryPathContext">The directory name the items was created from, will be used only when throwing an exception</param>
        public void AssertNotEmpty(string[] items, string directoryPathContext);
    }
}