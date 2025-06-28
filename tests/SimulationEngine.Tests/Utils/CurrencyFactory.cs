using IdelPog.SimulationEngine.Flows.Currency;

namespace IdelPogTests.Utils
{
    internal static class CurrencyFactory
    {
        internal static Currency CreateGems()
        {
            return new Currency(CurrencyType.GEMS, 0);
        }

        internal static Currency CreateGold()
        {
            return new Currency(CurrencyType.GOLD, 0);
        }
    }
}