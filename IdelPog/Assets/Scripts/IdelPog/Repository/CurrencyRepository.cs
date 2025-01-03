using System;
using System.Collections.Generic;
using IdelPog.Exceptions;
using IdelPog.Model;
using IdelPog.Structures;

namespace IdelPog.Repository
{
    /// <summary>
    /// This class stores all <see cref="Currency"/> models. Please see <see cref="ICurrencyRepository"/> for documentation.
    /// </summary>
    public class CurrencyRepository : ICurrencyRepository
    {
        protected Dictionary<CurrencyType, Currency> Repository = new();

        public bool Add(Currency currency)
        {
            if (currency == null)
            {
                throw new ArgumentNullException();
            }
            
            CurrencyType type = currency.CurrencyType;
            AssertTypeIsValid(type);
            
            bool contains = Repository.ContainsKey(type);
            if (contains)
            {
                throw new ArgumentException($"Error! A Currency with CurrencyType {type} already exists.");
            }
            
            Repository.Add(type, currency);
            return true;
        }

        public bool Remove(CurrencyType currencyType)
        {
            AssertTypeIsValid(currencyType);
            AssertCurrencyExists(currencyType);
            
            Repository.Remove(currencyType);
            return true;
        }

        public Currency Get(CurrencyType currencyType)
        {
            AssertTypeIsValid(currencyType);
            AssertCurrencyExists(currencyType);
            
            Currency currency = Repository[currencyType];
            return currency;
        }
        
        /// <summary>
        /// Asserts that the passed <see cref="CurrencyType"/> is not <see cref="CurrencyType.NO_TYPE"/>
        /// </summary>
        /// <param name="currencyType">The <see cref="CurrencyType"/> you want to check</param>
        /// <exception cref="NoTypeException">Will be thrown if the passed <see cref="CurrencyType"/> is <see cref="CurrencyType.NO_TYPE"/></exception>
        private static void AssertTypeIsValid(CurrencyType currencyType)
        {
            if (currencyType == CurrencyType.NO_TYPE)
            {
                throw new NoTypeException("Error! Passed CurrencyType is NO_TYPE, nothing can be retrieved. This should be fixed.");
            }
        }

        /// <summary>
        /// Asserts that the passed <see cref="CurrencyType"/> is in the Repository.
        /// This will throw an exception if the passed <see cref="CurrencyType"/> is not found
        /// </summary>
        /// <param name="currencyType">The <see cref="CurrencyType"/> you want to assure is in the Repository</param>
        /// <exception cref="NotFoundException">Will be thrown if the passed <see cref="CurrencyType"/> is not found</exception>
        private void AssertCurrencyExists(CurrencyType currencyType)
        {
            bool contains = Repository.ContainsKey(currencyType);
            if (contains == false)
            {
                throw new NotFoundException($"Error! Passed CurrencyType {currencyType} is not in the Repository.");
            }
        }
    }
}
