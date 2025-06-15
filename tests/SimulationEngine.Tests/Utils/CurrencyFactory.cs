using IdelPog.SimulationEngine.Models;
using IdelPog.SimulationEngine.Structures.Enums;

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