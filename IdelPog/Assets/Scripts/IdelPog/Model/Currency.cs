using IdelPog.Structures;

namespace IdelPog.Model
{
    /// <summary>
    /// The Currency model.
    /// </summary>
    public class Currency
    {
        private readonly CurrencyType _currencyType;

        public Currency(CurrencyType currencyType)
        {
            _currencyType = currencyType;
        }
    }
}