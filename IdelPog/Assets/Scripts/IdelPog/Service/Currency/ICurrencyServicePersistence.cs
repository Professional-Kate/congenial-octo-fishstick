using IdelPog.Structures;

namespace IdelPog.Service.Currency
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="ProcessCurrencyUpdate"/>
    public interface ICurrencyServicePersistence
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trades"></param>
        /// <returns></returns>
        public ServiceResponse ProcessCurrencyUpdate(params CurrencyTrade[] trades);
    }
}