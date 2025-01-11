using IdelPog.Orchestration;
using IdelPog.Structures;
using UnityEngine;

namespace IdelPog.Controller
{
    /// <summary>
    /// The main control object for Currency. Using this class you can Update any Currency.
    /// </summary>
    /// <seealso cref="UpdateCurrency"/>
    public class CurrencyController : Singleton<CurrencyController>, ICurrencyController
    {
        protected ICurrencyMediator CurrencyService = CurrencyMediator.CreateDefault();
        
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