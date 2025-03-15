using IdelPog.Model;
using IdelPog.Structures.Builders;
using IdelPog.Structures.Enums;

namespace Tests.Utils
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