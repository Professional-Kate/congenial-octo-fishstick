using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Currency.Factory.Interface
{
    public interface ICurrencyUpdateFactory
    {
        public CurrencyUpdate CreateCurrencyUpdate(ActionType actionType, uint amount, CurrencyType currencyType);
    }
}