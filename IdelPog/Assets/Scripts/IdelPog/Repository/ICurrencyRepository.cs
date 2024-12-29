using System;
using IdelPog.Exceptions;
using IdelPog.Model;
using IdelPog.Structures;

namespace IdelPog.Repository
{
    /// <summary>
    /// The purpose of this interface is to give access to Adding, Removing, and finally getting a <see cref="Currency"/> from an internal data structure
    /// </summary>
    public interface ICurrencyRepository
    {
        /// <summary>
        /// Adds a <see cref="Currency"/> model with a connected <see cref="CurrencyType"/> key into the Repository
        /// </summary>
        /// <param name="currency">The <see cref="Currency"/> model you want to add</param>
        /// <returns>if the operation was successful</returns>
        /// <exception cref="ArgumentNullException">If the passed <see cref="Currency"/> model is null</exception>
        /// <exception cref="NoTypeException">If the <see cref="Currency"/> model <see cref="CurrencyType"/> is <see cref="CurrencyType.NO_TYPE"/></exception>
        /// <exception cref="ArgumentException">If the passed <see cref="Currency"/> model already exists in the Repository</exception>
        /// <remarks>
        /// Every implementation of this method should follow these rules :
        /// <list type="bullet">
        /// <item> You should throw an exception if the passed <see cref="Currency"/> model <see cref="CurrencyType"/> is <see cref="CurrencyType.NO_TYPE"/></item>
        /// <item> The passed <see cref="Currency"/> will be linked to the <see cref="CurrencyType"/> key in the Repository </item>
        /// <item> Always ensure that this method will throw an exception if the passed <see cref="Currency"/> model already exists in the Repository </item>
        /// </list>
        /// </remarks>
        public bool Add(Currency currency);

        /// <summary>
        /// Removes a <see cref="Currency"/> model from the Repository by using its <see cref="CurrencyType"/>
        /// </summary>
        /// <param name="currencyType">The <see cref="Currency"/> model you want to remove should have this <see cref="CurrencyType"/></param>
        /// <returns>if the operation was successful</returns>
        /// <exception cref="NoTypeException">If the passed <see cref="CurrencyType"/> is <see cref="CurrencyType.NO_TYPE"/></exception>
        /// <exception cref="ArgumentException">If the passed <see cref="CurrencyType"/> is not in the Repository</exception>
        /// <remarks>
        /// Every implementation of this method should follow these rules :
        /// <list type="bullet">
        /// <item> You should throw an exception if the passed <see cref="CurrencyType"/> is <see cref="CurrencyType.NO_TYPE"/></item>
        /// <item> An exception should be thrown if the <see cref="Currency"/> is not found in the Repository</item>
        /// </list>
        /// </remarks>
        public bool Remove(CurrencyType currencyType);

        /// <summary>
        /// Returns an item from the Repository using a <see cref="CurrencyType"/> tag
        /// </summary>
        /// <param name="currencyType">The type of the <see cref="Currency"/> model you want to get</param>
        /// <returns>The found <see cref="Currency"/> model</returns>
        /// <exception cref="NoTypeException">If the passed <see cref="CurrencyType"/> is <see cref="CurrencyType.NO_TYPE"/></exception>
        /// /// <remarks>
        /// Every implementation of this method should follow these rules :
        /// <list type="bullet">
        /// <item> You should throw an exception if the passed <see cref="CurrencyType"/> is <see cref="CurrencyType.NO_TYPE"/></item>
        /// <item> You should throw an exception if the passed <see cref="CurrencyType"/> is not found </item>
        /// </list>
        /// </remarks>
        public Currency Get(CurrencyType currencyType);
    }
}