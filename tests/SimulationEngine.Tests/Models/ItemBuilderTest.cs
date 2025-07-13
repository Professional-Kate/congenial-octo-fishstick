using IdelPog.SimulationEngine.Constants;
using IdelPog.SimulationEngine.Inventory;
using IdelPog.SimulationEngine.Structures.Types;

namespace IdelPogTests.Models
{
    [TestFixture]
    public class ItemBuilderTest
    {
        private const ItemID INVENTORY_ID = ItemID.OAK_WOOD;
        private readonly Information _information = ItemConstants.OAK_WOOD;

        private IItemBuilder GetBuilder()
        {
            return ItemBuilder.Create(INVENTORY_ID, _information);
        }

        [Test]
        public void Positive_Create_AssignsData()
        {
            Item item = GetBuilder().Build();
            
            Assert.Multiple(() =>
            {
                Assert.That(item.ID, Is.EqualTo(INVENTORY_ID));
                Assert.That(item.Information, Is.EqualTo(_information));
            });
        }

        [Test]
        public void Positive_Create_AssignsDefaults()
        {
            Item item = GetBuilder().Build();
            
            Assert.Multiple(() =>
            {
                Assert.That(item.Amount, Is.Not.EqualTo(0));
                Assert.That(item.SellPrice, Is.Not.EqualTo(0));
            });
        }

        [Test]
        public void Positive_Build_BuildsData()
        {
            const int sellPrice = 100;
            const int amount = 100;
            
            Item item = GetBuilder()
                .SellPrice(sellPrice)
                .Amount(amount)
                .Build();
            
            Assert.Multiple(() =>
            {
                Assert.That(item.SellPrice, Is.EqualTo(sellPrice));
                Assert.That(item.Amount, Is.EqualTo(amount));
            });
        }
    }
}