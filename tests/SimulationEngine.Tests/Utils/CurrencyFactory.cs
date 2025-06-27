using IdelPog.SimulationEngine.Flows.Currency;

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
            return new Currency(CurrencyType.FOOD, 0);
        }
    }
}