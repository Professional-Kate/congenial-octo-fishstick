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
            throw new System.NotImplementedException();
        }

        public Currency Get(CurrencyType currencyType)
        {
            throw new System.NotImplementedException();
        }
    }
}
