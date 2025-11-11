using IdelPog.Currency.Contracts.Command;
using IdelPog.Currency.Contracts.Response;

namespace IdelPog.Currency.Factory.Interface
{
    public interface IItemBuyResponseFactory
    {
        public ItemBuyResponse Create(ItemBuy itemBuy);

        public ItemBuyResponse[] CreateMultiple(IReadOnlyList<ItemBuy> itemBuys);
    }
}