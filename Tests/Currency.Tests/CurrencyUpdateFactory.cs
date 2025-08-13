using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Currency.Tests
{
    public static class CurrencyUpdateFactory
    {
        public static CurrencyUpdate Create(uint amount, CurrencyType currencyType, ActionType actionType)
        {
            return new CurrencyUpdate
            {
                Amount = amount,
                CurrencyType = currencyType,
                ActionType = actionType
            };
        }
    }
}