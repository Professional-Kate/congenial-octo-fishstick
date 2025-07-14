using IdelPog.Common.Enums;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Structures;

namespace IdelPog.Common.Factories
{
    public interface ICurrencyUpdateFactory
    {
        public CurrencyUpdate CreateCurrencyUpdate(ActionType actionType, int amount, CurrencyType currencyType);
    }
}