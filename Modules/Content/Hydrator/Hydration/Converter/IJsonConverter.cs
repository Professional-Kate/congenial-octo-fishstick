using System.Text.Json;

namespace IdelPog.Content.Hydrator.Hydration.Converter
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
        /// <param name="jsonDocument">The JSON document to convert</param>
        /// <returns>A new instance of type <typeparamref name="T"/></returns>
        public T Convert(JsonDocument jsonDocument);
    }
}