using IdelPog.Common.Structures;
using IdelPog.SimulationEngine.Structures.Enums;

namespace IdelPog.SimulationEngine.Models
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

        public Currency DeepClone()
        {
            return new Currency(CurrencyType, Amount);
        }
    }
}