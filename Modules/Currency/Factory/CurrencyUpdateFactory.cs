using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Currency.Factory.Interface;

namespace IdelPog.Currency.Factory
{
    public class CurrencyUpdateFactory : ICurrencyUpdateFactory
    {
        public CurrencyUpdate CreateCurrencyUpdate(ActionType actionType, uint amount, CurrencyType currencyType)
        {
            return new CurrencyUpdate { ActionType = actionType, Amount = amount, CurrencyType = currencyType };
        }
    }
}