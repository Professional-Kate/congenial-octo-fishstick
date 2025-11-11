using IdelPog.Core.Contracts.Enum;
using IdelPog.Currency.Contracts.Command;
using IdelPog.Currency.Contracts.Error;
using IdelPog.Currency.Factory;

namespace IdelPog.Currency.Tests.Factory
{
    [TestFixture]
    public sealed class ItemBuyResponseFactoryTest
    {
        private ItemBuyResponseFactory _itemBuyResponseFactory;

        private ItemBuy _honeyBuy;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _itemBuyResponseFactory = new ItemBuyResponseFactory();

            _honeyBuy = new ItemBuy { CurrencyType = CurrencyType.GOLD, ItemID = ItemID.HONEY, Amount = 5, Price = 3 };
        }

        private static void AssertItemBuyResponse(ItemBuyResponse itemBuyResponse, ItemBuy itemBuy)
        {
            Assert.Multiple(() =>
            {
                Assert.That(itemBuyResponse.CurrencyType, Is.EqualTo(itemBuy.CurrencyType));
                Assert.That(itemBuyResponse.ItemID, Is.EqualTo(itemBuy.ItemID));
                Assert.That(itemBuyResponse.Price, Is.EqualTo(itemBuy.Price));
                Assert.That(itemBuyResponse.Amount, Is.EqualTo(itemBuy.Amount));
            });
        }

        [Test]
        public void Positive_Create_TransformsItemBuy_IntoResponse()
        { 
            ItemBuyResponse response = _itemBuyResponseFactory.Create(_honeyBuy);
            
            AssertItemBuyResponse(response, _honeyBuy);
        }

        [Test]
        public void Positive_CreateMultiple_TransformCollection()
        {
            IReadOnlyList<ItemBuyResponse> response = _itemBuyResponseFactory.CreateMultiple([_honeyBuy, _honeyBuy, _honeyBuy]);

            Assert.That(response, Has.Count.EqualTo(3));
            foreach (ItemBuyResponse itemBuyResponse in response)
            { 
                AssertItemBuyResponse(itemBuyResponse, _honeyBuy);
            }
        }
    }
}