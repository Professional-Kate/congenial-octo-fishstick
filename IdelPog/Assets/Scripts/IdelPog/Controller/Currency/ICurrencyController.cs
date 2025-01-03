using IdelPog.Structures;

namespace IdelPog.Controller.Currency
{
    public interface ICurrencyController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trades"></param>
        public void ProcessCurrencyUpdate(CurrencyTrade[] trades);
    }
}