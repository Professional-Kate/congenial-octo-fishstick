using IdelPog.Core.Factory.Interface;
using IdelPog.Inventory.Contracts.Command;
using IdelPog.Inventory.Contracts.Error;

namespace IdelPog.Inventory.Factory
{
    public sealed class ItemSellErrorFactory : IErrorFactory<ItemSellError, IReadOnlyList<ItemSell>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public ItemSellErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public ItemSellError Create<TException>(TException exception, IReadOnlyList<ItemSell> context) where TException : Exception
        {
            return new ItemSellError
            {
                ItemSells = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}