using IdelPog.Common.Enums;

namespace IdelPog.Common.Factories
{
    public interface ICurrencyUpdateFactory
    {
        public CurrencyUpdate CreateCurrencyUpdate(ActionType actionType, int amount, CurrencyType currencyType);
    }
}