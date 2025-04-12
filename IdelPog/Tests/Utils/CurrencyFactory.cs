using IdelPogTemp.Main.Structures.Enums;
using IdelPogTemp.Main.Structures.Models;
using IdelPogTemp.Main.Structures.Models.Builders.Currency;

namespace IdelPogTemp.Tests.Utils
{
    internal static class CurrencyFactory
    {
        internal static Currency CreateWood()
        {
            return CurrencyBuilder.Builder()
                .CurrencyType(CurrencyType.WOOD)
                .Amount(0)
                .Build();
        }

        internal static Currency CreateFood()
        {
            return CurrencyBuilder.Builder()
                .CurrencyType(CurrencyType.FOOD)
                .Amount(0)
                .Build();
        }
    }
}