using IdelPog.Orchestration.Currency;
using IdelPog.Service;
using IdelPog.Structures;
using UnityEngine;

namespace IdelPog.Controller.Currency
{
    /// <summary>
    /// The main control object for Currency. Using this class you can Update any Currency.
    /// </summary>
    /// <seealso cref="UpdateCurrency"/>
    public class CurrencyController : Singleton<CurrencyController>, ICurrencyController
    {
        protected ICurrencyMediator CurrencyService = new CurrencyMediator();
        
        public void UpdateCurrency(params CurrencyTrade[] trades)
        {
            ServiceResponse serviceResponse = CurrencyService.ProcessCurrencyUpdate(trades);
            if (serviceResponse.IsSuccess == false)
            {
                // TODO: logger log. 
                Debug.Log(serviceResponse.Message);
            }
        }
    }
} 