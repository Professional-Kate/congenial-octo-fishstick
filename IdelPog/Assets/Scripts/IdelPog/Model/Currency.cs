using IdelPog.Structures;

namespace IdelPog.Model
{
    /// <summary>
    /// The Currency model.
    /// </summary>
    public class Currency
    {
        public readonly CurrencyType CurrencyType;
        public int Amount { get; private set; }

        public Currency(CurrencyType currencyType)
        {
            CurrencyType = currencyType;
        }

        public void SetAmount(int amount)
        {
            Amount = amount;
        }
    }
}