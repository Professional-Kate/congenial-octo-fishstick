using IdelPog.Engine.Structures.Enums;
using IdelPog.Engine.Structures.Models;
using IdelPog.Engine.Utilities.Builders.Currency;

namespace IdelPog.Tests.Utils
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