using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency
{
    public interface ICurrencyController
    {
        /// <summary>
        /// Uses a passed <see cref="CurrencyTrade"/> to dictate how to update a Currency model with matching <see cref="CurrencyType"/>
        /// </summary>
        /// <param name="trades">A <see cref="IReadOnlyList{T}"/> containing all the trades you want processed</param>
        public void UpdateCurrency(IReadOnlyList<CurrencyTrade> trades);
        
        public void CreateCurrency(IReadOnlyList<CurrencyCreation> commands);
    }
}