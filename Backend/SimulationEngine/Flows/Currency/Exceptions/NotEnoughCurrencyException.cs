using IdelPog.Common.Enums;

namespace IdelPog.SimulationEngine.Currency.Exceptions
{
    public class NotEnoughCurrencyException : Exception
    {
        private const string MESSAGE = "CurrencyType: {0} doesn't have enough amount to remove! Currency amount {1}, needed amount {2}";

        public readonly CurrencyType CurrencyTypeContext;
        public readonly int CurrencyAmount;
        public readonly int RemoveAmount;

        public NotEnoughCurrencyException(CurrencyType currencyTypeContext, int currencyAmount, int removeAmount) : base(string.Format(MESSAGE,
            currencyTypeContext.ToString(), currencyAmount, removeAmount))
        {
            CurrencyTypeContext = currencyTypeContext;
            CurrencyAmount = currencyAmount;
            RemoveAmount = removeAmount;
        }
    }
}