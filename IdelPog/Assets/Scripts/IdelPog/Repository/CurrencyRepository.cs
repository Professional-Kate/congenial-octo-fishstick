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
        private Dictionary<CurrencyType, Currency> _repository = new();

        public bool Add(CurrencyType currencyType, Currency currency)
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
