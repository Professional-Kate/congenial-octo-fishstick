using System.Text.Json;

namespace ContentHydrator.Converters
{
    /// <summary>
    /// converts JSON strings into the specified DTO 
    /// </summary>
    /// <typeparam name="T">The type of the DTO</typeparam>
    public interface IJsonConverter<out T>
    {
        /// <summary>
        /// Converts given JSON documents into a new instance of <typeparamref name="T"/>
        /// </summary>
        /// <param name="jsonString">The JSON document to convert</param>
        /// <returns>A new instance of type <typeparamref name="T"/></returns>
        public T Convert(JsonDocument jsonString);
    }
}