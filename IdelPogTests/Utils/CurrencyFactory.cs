using IdelPog.Engine.Structures.Enums;
using IdelPog.Engine.Structures.Models;

namespace IdelPogTests.Utils
{
    internal static class CurrencyFactory
    {
        internal static Currency CreateWood()
        {
            return new Currency(CurrencyType.WOOD, 0);
        }

        internal static Currency CreateFood()
        {
            return new Currency(CurrencyType.FOOD);
        }
    }
}