using IdelPog.Model;
using IdelPog.Structures.Enums;

namespace IdelPog.Structures.Builders
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

        public Currency Build()
        {
            return new Currency(_currencyType, _amount);
        }
    }
}