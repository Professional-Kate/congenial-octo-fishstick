using System;
using System.Collections.Generic;
using IdelPog.Model;
using IdelPog.Structures;

namespace IdelPog.Repository
{
    /// <summary>
    /// This class stores all <see cref="Currency"/> models.
    /// </summary>
    public class CurrencyRepository : ICurrencyRepository
    {
        protected Dictionary<CurrencyType, Currency> Repository = new();

        public bool Add( Currency currency)
        {
            if (currency == null)
            {
                throw new ArgumentNullException();
            }
            
            CurrencyType type = currency.CurrencyType;

            if (type == CurrencyType.NO_TYPE)
            {
                throw new ArgumentException("Error! Passed CurrencyType is NO_TYPE, nothing has been added. This should be fixed.");
            }
            
            Repository.Add(type, currency);
            return true;
        }

        public bool Remove(CurrencyType currencyType)
        {
            if (currencyType == CurrencyType.NO_TYPE)
            {
                throw new ArgumentException("Error! Passed CurrencyType is NO_TYPE, nothing can be removed. This should be fixed.");
            }
            
            bool containsKey = Repository.ContainsKey(currencyType);
            if (containsKey == false)
            {
                throw new ArgumentException("Error! Passed CurrencyType is not in the Repository.");
            }
            
            Repository.Remove(currencyType);
            return true;
        }

        public Currency Get(CurrencyType currencyType)
        {
            if (currencyType == CurrencyType.NO_TYPE)
            {
                throw new ArgumentException("Error! Passed CurrencyType is NO_TYPE, nothing can be retrieved.");
            }
            
            Currency currency = Repository[currencyType];
            
            return currency;
        }
    }
}
