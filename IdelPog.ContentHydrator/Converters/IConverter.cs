namespace ContentHydrator.Converters
{
    /// <summary>
    /// converts strings into the specified DTO 
    /// </summary>
    /// <typeparam name="T">The type of the DTO</typeparam>
    public interface IConverter<out T>
    {
        /// <summary>
        /// Converts given strings into a new instance of <typeparamref name="T"/>
        /// </summary>
        /// <param name="content">The string to convert</param>
        /// <returns>A new instance of type <typeparamref name="T"/></returns>
        public T Convert(string content);
    }
}