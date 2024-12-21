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

        public bool Add(Currency currency)
        {
            throw new System.NotImplementedException();
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
