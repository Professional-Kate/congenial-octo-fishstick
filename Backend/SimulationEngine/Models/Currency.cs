using IdelPog.Common.Enums;
using IdelPog.Common.Structures;

namespace IdelPog.SimulationEngine.Models
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