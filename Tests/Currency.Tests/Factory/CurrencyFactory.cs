using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Currency.Tests.Factory
{
    internal static class CurrencyFactory
    {
        internal static IdelPog.Currency.Contracts.Currency CreateGems()
        {
            return new IdelPog.Currency.Contracts.Currency(CurrencyType.GEMS, 0);
        }

        internal static IdelPog.Currency.Contracts.Currency CreateGold()
        {
            return new IdelPog.Currency.Contracts.Currency(CurrencyType.GOLD, 0);
        }
    }
}