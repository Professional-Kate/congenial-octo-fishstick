using IdelPog.Service;
using IdelPog.Service.Currency;
using IdelPog.Structures;
using UnityEngine;

namespace IdelPog.Controller.Currency
{
    public class CurrencyController : ICurrencyController
    {
        protected ICurrencyMediator CurrencyService = new CurrencyService();
        
        public void ProcessCurrencyUpdate(params CurrencyTrade[] trades)
        {
            ServiceResponse serviceResponse = CurrencyService.ProcessCurrencyUpdate(trades);
            if (serviceResponse.IsSuccess == false)
            {
                Debug.Log(serviceResponse.Message);
            }
        }
    }
} 