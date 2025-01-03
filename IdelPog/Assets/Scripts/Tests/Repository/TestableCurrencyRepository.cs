using System.Collections.Generic;
using IdelPog.Model;
using IdelPog.Repository;
using IdelPog.Repository.Currency;
using IdelPog.Structures;

namespace Tests.Repository
{
    /// <summary>
    /// Just for testing. The purpose of this class is to act as a dependency injector for the internal data structure to allow for better testing.
    /// </summary>
    internal class TestableCurrencyRepository : CurrencyRepository
    {
        internal TestableCurrencyRepository(Dictionary<CurrencyType, Currency> currencies)
        {
            Repository = currencies;
        }
    }
}