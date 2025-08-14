using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Currency.Contracts
{
    public sealed class Currency : ICloneable<Currency>
    {
        public readonly CurrencyType CurrencyType;
        public uint Amount { get; set; }

        public Currency(CurrencyType currencyType, uint amount)
        {
            CurrencyType = currencyType;
            Amount = amount;
        }

        public Currency DeepClone()
        {
            return new Currency(CurrencyType, Amount);
        }
    }
}