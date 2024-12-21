using IdelPog.Structures;

namespace IdelPog.Model
{
    /// <summary>
    /// The Currency model.
    /// </summary>
    public class Currency
    {
        public readonly CurrencyType CurrencyType;

        public Currency(CurrencyType currencyType)
        {
            CurrencyType = currencyType;
        }
    }
}