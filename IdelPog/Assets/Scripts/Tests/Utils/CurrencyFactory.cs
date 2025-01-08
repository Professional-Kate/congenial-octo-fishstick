using IdelPog.Model;
using IdelPog.Structures;

namespace Tests.Utils
{
    internal class CurrencyFactory
    {
        internal static Currency CreateWood()
        {
            return new Currency(CurrencyType.WOOD);
        }

        internal static Currency CreateFood()
        {
            return new Currency(CurrencyType.FOOD);
        }
    }
}