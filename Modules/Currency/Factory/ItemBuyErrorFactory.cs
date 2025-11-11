using IdelPog.Core.Factory.Interface;
using IdelPog.Currency.Contracts.Command;
using IdelPog.Currency.Contracts.Error;

namespace IdelPog.Currency.Factory
{
    public sealed class ItemBuyErrorFactory : IErrorFactory<ItemBuyError, IReadOnlyList<ItemBuy>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public ItemBuyErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }
        
        public ItemBuyError Create<TException>(TException exception, IReadOnlyList<ItemBuy> context) where TException : Exception
        {
            return new ItemBuyError
            {
                ItemBuys = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}