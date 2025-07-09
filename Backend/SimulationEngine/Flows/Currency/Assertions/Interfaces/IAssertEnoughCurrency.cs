namespace IdelPog.SimulationEngine.Currency.Assertions
{
    public interface IAssertEnoughCurrency
    {
        public void Handle(int currencyAmount, int removeAmount, CurrencyType currencyTypeContext);
    }
}