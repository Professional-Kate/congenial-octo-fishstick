using IdelPog.Model;
using IdelPog.Structures;

namespace IdelPog.Repository
{
    /// <summary>
    /// The purpose of this interface is to give access to Adding, Removing, and finally getting an object from an internal data structure
    /// </summary>
    public interface ICurrencyRepository
    {
        // TODO: When tests are added update these doc comments with exactly what exceptions they can throw and any remarks of each method.
        
        /// <summary>
        /// Adds a <see cref="Currency"/> model with a connected <see cref="CurrencyType"/> key into the Repository
        /// </summary>
        /// <param name="currencyType">The <see cref="CurrencyType"/> you want this <see cref="Currency"/> to be referenced by</param>
        /// <param name="currency">The <see cref="Currency"/> model you want to add</param>
        /// <returns>if the operation was successful</returns>
        public bool Add(CurrencyType currencyType, Currency currency);

        /// <summary>
        /// Removes a <see cref="Currency"/> model from the Repository by using its <see cref="CurrencyType"/>
        /// </summary>
        /// <param name="currencyType">The <see cref="Currency"/> model you want to remove should have this <see cref="CurrencyType"/></param>
        /// <returns>if the operation was successful</returns>
        public bool Remove(CurrencyType currencyType);

        /// <summary>
        /// Returns an item from the Repository using a <see cref="CurrencyType"/> tag
        /// </summary>
        /// <param name="currencyType">The type of the <see cref="Currency"/> model you want to get</param>
        /// <returns>The found <see cref="Currency"/> model</returns>
        public Currency Get(CurrencyType currencyType);
    }
}