using IdelPog.Model;
using IdelPog.Structures.Enums;

namespace Tests.Utils
{
    internal static class CurrencyFactory
    {
        internal static Currency CreateGold()
        {
            return new Currency(CurrencyType.GOLD);
        }

        internal static Currency CreatePeople()
        {
            return new Currency(CurrencyType.PEOPLE);
        }
    }
}