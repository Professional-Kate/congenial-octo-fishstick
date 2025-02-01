using System;
using System.Collections.Generic;
using IdelPog.Exceptions;
using IdelPog.Service;
using IdelPog.Structures;
using IdelPog.Structures.Item;
using NUnit.Framework;

namespace Tests.Service
{
    [TestFixture]
    public class ItemFactoryTest
    {
        private IItemFactory _itemFactory { get; set; }

        private const InventoryID ID = InventoryID.OAK_WOOD;
        private readonly Information INFORMATION = Information.Create("Test", "ing");
        private const int SELL_PRICE = 1;
        private const int AMOUNT = 1;

        [SetUp]
        public void Setup()
        {
            _itemFactory = new ItemFactory();
        }

        [Test]
        public void Positive_CreateItem_CreatesItem()
        {
            Item createdItem = _itemFactory.CreateItem(ID, INFORMATION, SELL_PRICE, AMOUNT);
            
            Assert.IsNotNull(createdItem);
            Assert.AreEqual(ID, createdItem.ID);
            Assert.AreEqual(INFORMATION, createdItem.Information);
            Assert.AreEqual(SELL_PRICE, createdItem.SellPrice);
            Assert.AreEqual(AMOUNT, createdItem.Amount);
        }

        [Test]
        public void Negative_CreateItem_NoType_Throws()
        {
            Assert.Throws<NoTypeException>(() => _itemFactory.CreateItem(InventoryID.NO_TYPE, INFORMATION, SELL_PRICE, AMOUNT));
        }

        private static IEnumerable<Information> CreateBadInformation()
        {
            return new[]
            {
                Information.Create("", "description"),
                Information.Create("name", ""),
                Information.Create("", ""),
                Information.Create("name", " "),
                Information.Create(" ", "description"),
                Information.Create(" ", " "),
                Information.Create(null, " "),
                Information.Create(" ", null),
                Information.Create(null, null)

            };
        }

        [TestCaseSource(nameof(CreateBadInformation))]
        public void Negative_CreateItem_EmptyInformation_Throws(Information information)
        {
            Assert.Throws<ArgumentException>(() => _itemFactory.CreateItem(ID, information, SELL_PRICE, AMOUNT));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Negative_CreateItem_BadSellPrice_Throws(int sellPrice)
        {
            Assert.Throws<ArgumentException>(() => _itemFactory.CreateItem(ID, INFORMATION, sellPrice, AMOUNT));
        }
        
        [TestCase(0)]
        [TestCase(-1)]
        public void Negative_CreateItem_BadStartingAmount_Throws(int amount)
        {
            Assert.Throws<ArgumentException>(() => _itemFactory.CreateItem(ID, INFORMATION, SELL_PRICE, amount));
        }
    }
}