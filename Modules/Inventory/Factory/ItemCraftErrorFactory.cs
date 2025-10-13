using IdelPog.Core.Factory.Interface;
using IdelPog.Inventory.Contracts.Command;
using IdelPog.Inventory.Contracts.Error;

namespace IdelPog.Inventory.Factory
{
    public sealed class ItemCraftErrorFactory : IErrorFactory<ItemCraftError, IReadOnlyList<ItemCraft>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public ItemCraftErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            this._baseErrorFactory = baseErrorFactory;
        }

        public ItemCraftError Create<TException>(TException exception, IReadOnlyList<ItemCraft> context) where TException : Exception
        {
            return new ItemCraftError
            {
                ItemCrafts = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}