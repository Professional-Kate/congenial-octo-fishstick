using IdelPog.Common.Commands;
using IdelPog.Common.Enums;

namespace IdelPog.Common.Factories
{
    public class CurrencyUpdateFactory : ICurrencyUpdateFactory
    {
        public CurrencyUpdate CreateCurrencyUpdate(ActionType actionType, uint amount, CurrencyType currencyType)
        {
            return new CurrencyUpdate { Action = actionType, Amount = amount, CurrencyType = currencyType };
        }
    }
}