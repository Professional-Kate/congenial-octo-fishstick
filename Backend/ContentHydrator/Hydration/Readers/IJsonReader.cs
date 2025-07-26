using System.Text.Json;

namespace IdelPog.ContentHydrator.Readers
{
    /// <summary>
    /// Represents a file reader that reads flat files and returns the content as a string
    /// </summary>
    public interface IJsonReader
    {
        /// <summary>
        /// Reads from the passed file, returning the file as a string
        /// </summary>
        /// <param name="filePath">The path of the file to be read</param>
        /// <returns>The file converted into a string</returns>
        public JsonDocument Read(string filePath);
    }
}