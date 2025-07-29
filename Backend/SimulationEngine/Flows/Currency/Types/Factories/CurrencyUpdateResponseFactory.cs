using IdelPog.Common.Commands;
using IdelPog.SimulationEngine.Currency.Responses;
using IdelPog.Validation.Assertions;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public class CurrencyUpdateResponseFactory : ICurrencyUpdateResponseFactory
    {
        private readonly IObjectNullAssertion _objectNullAssertion;
        private readonly ICollectionAssertion _collectionAssertion;

        public CurrencyUpdateResponseFactory(IObjectNullAssertion objectNullAssertion, ICollectionAssertion collectionAssertion)
        {
            _objectNullAssertion = objectNullAssertion;
            _collectionAssertion = collectionAssertion;
        }

        public CurrencyUpdateResponse[] CreateFrom(IReadOnlyList<CurrencyUpdate> trades)
        {
            _objectNullAssertion.AssertNotNull(trades, nameof(trades));
            _collectionAssertion.AssertNotEmpty(trades);

            List<CurrencyUpdateResponse> result = new(trades.Count);

            foreach (CurrencyUpdate currencyTrade in trades)
            {
                result.Add(Create(currencyTrade));
            }

            return result.ToArray();
        }
        
        public CurrencyUpdateResponse CreateFrom(CurrencyUpdate trade)
        {
            _objectNullAssertion.AssertNotNull(trade, nameof(trade));

            return Create(trade);
        }

        private static CurrencyUpdateResponse Create(CurrencyUpdate trade)
        {
            return new CurrencyUpdateResponse
            {
                Action = trade.Action,
                Amount = trade.Amount,
                CurrencyType = trade.CurrencyType
            };
        }
    }
}