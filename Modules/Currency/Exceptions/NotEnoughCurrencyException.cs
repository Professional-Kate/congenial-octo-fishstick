using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Currency.Exceptions
{
    public class NotEnoughCurrencyException : Exception
    {
        private const string MESSAGE = "CurrencyType: {0} doesn't have enough amount to remove! Currency amount {1}, needed amount {2}";

        public readonly CurrencyType CurrencyTypeContext;
        public readonly uint CurrencyAmount;
        public readonly uint RemoveAmount;

        public NotEnoughCurrencyException(CurrencyType currencyTypeContext, uint currencyAmount, uint removeAmount) : base(string.Format(MESSAGE,
            currencyTypeContext.ToString(), currencyAmount, removeAmount))
        {
            CurrencyTypeContext = currencyTypeContext;
            CurrencyAmount = currencyAmount;
            RemoveAmount = removeAmount;
        }
    }
}