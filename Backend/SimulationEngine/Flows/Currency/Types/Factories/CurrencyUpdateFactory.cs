using IdelPog.Common.Commands;
using IdelPog.Common.Enums;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public class CurrencyUpdateFactory : ICurrencyUpdateFactory
    {
        public CurrencyUpdate CreateCurrencyUpdate(CurrencyType currencyType, ActionType actionType, int amount)
        {
            return new CurrencyUpdate
            {
                Action = actionType,
                Amount = amount,
                CurrencyType = currencyType
            };
        }
    }
}