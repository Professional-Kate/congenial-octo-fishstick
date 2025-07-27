using IdelPog.Common.Commands;
using IdelPog.Common.Enums;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public interface ICurrencyUpdateFactory
    {
        public CurrencyUpdate CreateCurrencyUpdate(CurrencyType currencyType, ActionType actionType, uint amount);
    }
}