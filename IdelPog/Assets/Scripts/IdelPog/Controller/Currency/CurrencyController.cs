using IdelPog.Service.Currency;
using IdelPog.Structures;

namespace IdelPog.Controller.Currency
{
    public class CurrencyController : ICurrencyController
    {
        protected ICurrencyService CurrencyService = new CurrencyService();
        
        public void ProcessCurrencyUpdate(params CurrencyTrade[] trades)
        {
            foreach (CurrencyTrade currencyTrade in trades)
            {
                switch (currencyTrade.Action)
                {
                    // TODO: add logging. 
                    case ActionType.ADD:
                        CurrencyService.AddAmount(currencyTrade.Currency, currencyTrade.Amount);
                        break;
                    case ActionType.REMOVE:
                        CurrencyService.RemoveAmount(currencyTrade.Currency, currencyTrade.Amount);
                        break;
                }
            }
        }
    }
} 