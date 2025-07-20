using IdelPog.Common.Enums;
using IdelPog.Common.Structures;

namespace IdelPog.SimulationEngine.Currency
{
    /// <summary>
    /// The Currency model.
    /// </summary>
    public class Currency: ICloneable<Currency>
    {
        public readonly CurrencyType CurrencyType;
        public int Amount { get; private set; }

        public Currency(CurrencyType currencyType, int amount)
        {
            CurrencyType = currencyType;
            Amount = amount;
        }

        public void SetAmount(int amount)
        {
            Amount = amount;
        }

        public Currency DeepClone()
        {
            return new Currency(CurrencyType, Amount);
        }
    }
}