using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Structures;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public interface ICurrencyUpdateFactory
    {
        public CurrencyUpdate CreateCurrencyUpdate(CurrencyType currencyType, ActionType actionType, int amount);
    }
}