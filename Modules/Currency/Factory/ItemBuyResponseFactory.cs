using IdelPog.Currency.Contracts.Command;
using IdelPog.Currency.Contracts.Response;
using IdelPog.Currency.Factory.Interface;

namespace IdelPog.Currency.Factory
{
    public sealed class ItemBuyResponseFactory : IItemBuyResponseFactory
    {
        public ItemBuyResponse Create(ItemBuy itemBuy)
        {
            return new ItemBuyResponse
            {
                CurrencyType = itemBuy.CurrencyType,
                ItemID = itemBuy.ItemID,
                Price = itemBuy.Price,
                Amount = itemBuy.Amount
            };
        }

        public ItemBuyResponse[] CreateMultiple(IReadOnlyList<ItemBuy> itemBuys)
        {
            ItemBuyResponse[] responses = new ItemBuyResponse[itemBuys.Count];
            for (int i = 0; i < itemBuys.Count; i++)
            { 
                responses[i] = Create(itemBuys[i]);
            }
            
            return responses;
        }
    }
}