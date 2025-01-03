using IdelPog.Service.Currency;
using IdelPog.Structures;

namespace IdelPog.Controller.Currency
{
    public class CurrencyController : ICurrencyController
    {
        protected ICurrencyService CurrencyService = new CurrencyService();
        
        public void ProcessCurrencyUpdate(CurrencyTrade[] trades)
        {
            throw new System.NotImplementedException();
        }
    }
}