using IdelPog.Engine.Structures.Enums;
using IdelPog.Engine.Structures.Types;

namespace IdelPog.Engine.Structures.Models
{
    /// <summary>
    /// The Currency model.
    /// </summary>
    public class Currency(CurrencyType currencyType, int amount = 0) : ICloneable<Currency>
    {
        public readonly CurrencyType CurrencyType = currencyType;
        public int Amount { get; private set; } = amount;

        public void SetAmount(int amount)
        {
            Amount = amount;
        }

        public Currency Clone()
        {
            return new Currency(CurrencyType, Amount);
        }
    }
}