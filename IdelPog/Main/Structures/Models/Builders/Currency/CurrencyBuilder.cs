using IdelPog.Main.Structures.Enums;

namespace IdelPog.Main.Structures.Models.Builders.Currency
{
    /// <inheritdoc cref="ICurrencyBuilder"/>
    public class CurrencyBuilder : ICurrencyBuilder
    {
        private CurrencyType _currencyType { get; set; }
        private int _amount { get; set; }
        
        public static ICurrencyBuilder Builder() => new CurrencyBuilder();

        public ICurrencyBuilder CurrencyType(CurrencyType currencyType)
        {
            _currencyType = currencyType;

            return this;
        }

        public ICurrencyBuilder Amount(int amount)
        {
            _amount = amount;

            return this;
        }

        public Models.Currency Build()
        {
            return new Models.Currency(_currencyType, _amount);
        }
    }
}