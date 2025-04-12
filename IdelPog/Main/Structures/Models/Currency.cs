using IdelPogTemp.Main.Structures.Enums;

namespace IdelPogTemp.Main.Structures.Models
{
    /// <summary>
    /// The Currency model.
    /// </summary>
    public class Currency : ICloneable
    {
        public readonly CurrencyType CurrencyType;
        public int Amount { get; private set; }

        public Currency(CurrencyType currencyType, int amount = 0)
        {
            CurrencyType = currencyType;
            Amount = amount;
        }

        public void SetAmount(int amount)
        {
            Amount = amount;
        }

        public object Clone()
        {
            return new Currency(CurrencyType, Amount);
        }
    }
}