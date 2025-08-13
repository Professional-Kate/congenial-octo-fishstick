using IdelPog.Content.Hydrator.Hydration.Converter;

namespace IdelPog.Content.Hydrator.Hydration.Provider
{
    /// <summary>
    /// Provides a new <see cref="IJsonConverter{T}"/> based on the type param T
    /// </summary>
    public interface IConverterProvider
    {
        /// <summary>
        /// Creates and returns a new <see cref="IJsonConverter{T}"/> for the type of T
        /// </summary>
        /// <typeparam name="T">The type of the converter</typeparam>
        /// <returns>The newly constructed converter</returns>
        public IJsonConverter<T> CreateConverter<T>();
    }
}