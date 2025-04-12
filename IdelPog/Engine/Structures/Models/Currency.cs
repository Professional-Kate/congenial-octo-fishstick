using IdelPog.Engine.Structures.Enums;

namespace IdelPog.Engine.Structures
{
    /// <summary>
    /// The Currency model.
    /// </summary>
    public class Currency(CurrencyType currencyType, int amount = 0) : ICloneable
    {
        public readonly CurrencyType CurrencyType = currencyType;
        public int Amount { get; private set; } = amount;

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