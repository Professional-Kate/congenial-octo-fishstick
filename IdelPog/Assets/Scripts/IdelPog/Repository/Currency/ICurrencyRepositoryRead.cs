using IdelPog.Exceptions;
using IdelPog.Model;
using IdelPog.Structures.Enums;

namespace IdelPog.Repository
{
    /// <summary>
    /// Allows Get access into the Repository
    /// </summary>
    /// <seealso cref="Get"/>
    public interface ICurrencyRepositoryRead
    {
        /// <summary>
        /// Returns an item from the Repository using its <see cref="CurrencyType"/> type. This <see cref="Currency"/> will be returned by value
        /// </summary>
        /// <param name="currencyType">The type of the <see cref="Currency"/> model you want to get</param>
        /// <returns>The found <see cref="Currency"/> model that is associated with the specific <see cref="CurrencyType"/></returns>
        /// <exception cref="NoTypeException">If the passed <see cref="CurrencyType"/> is <see cref="CurrencyType.NO_TYPE"/></exception>
        /// <exception cref="NotFoundException">If the passed <see cref="CurrencyType"/> is not found within the Repository</exception>
        /// <remarks>
        /// The returned <see cref="Currency"/> will be passed by value. So any changes to the returned <see cref="Currency"/> will not change that <see cref="Currency"/> in the Repository
        /// </remarks>
        public Currency Get(CurrencyType currencyType);
    }
}