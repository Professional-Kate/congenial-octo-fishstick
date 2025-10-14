using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Service;
using Moq;

namespace IdelPog.Inventory.Tests.Service
{
    [TestFixture]
    public sealed class ItemCreationServiceTest
    {
        private ItemCreationService _itemCreationService;
        private Mock<IAssetRepository<ItemID, ItemDefinition>> _repositoryMock;

        private ItemDefinition _stoneDefinition;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _stoneDefinition = new ItemDefinition
            {
                ItemID = ItemID.STONE,
                BaseSellPrice = 1,
                Information = new Information { Name = "Stone", Description = "Hard" }
            };

            _repositoryMock = new Mock<IAssetRepository<ItemID, ItemDefinition>>();

            _itemCreationService = new ItemCreationService(_repositoryMock.Object, new FoundAssertion(new ThrowHandler()));
        }

        [SetUp]
        public void Setup()
        {
            _repositoryMock.Reset();
        }

        private void SetupRepositoryContains(bool contains, ItemID itemID)
        {
            _repositoryMock.Setup(library => library.Contains(itemID)).Returns(contains);
        }

        private void SetupRepositoryGet(ItemDefinition definition, ItemID itemID)
        {
            _repositoryMock.Setup(library => library.Get(itemID)).Returns(definition);
        }

        private static void AssertItem(Item item, ItemDefinition definition, uint amount)
        {
            Assert.Multiple(() =>
            {
                Assert.That(item.ItemID, Is.EqualTo(definition.ItemID));
                Assert.That(item.Information, Is.EqualTo(definition.Information));
                Assert.That(item.BaseSellPrice, Is.EqualTo(definition.BaseSellPrice));
                Assert.That(item.Amount, Is.EqualTo(amount));
            });
        }

        private void VerifyRepositoryCalled(ItemID itemID)
        { 
            _repositoryMock.Verify(library => library.Contains(itemID), Times.Once);
            _repositoryMock.Verify(library => library.Get(itemID), Times.Once);
            VerifyRepositoryNoOtherCalls();
        }

        private void VerifyRepositoryNoOtherCalls()
        {
            _repositoryMock.VerifyNoOtherCalls();
        }

        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(uint.MaxValue)]
        public void Positive_Create_DifferentAmounts_CreatesItem(uint amount)
        {
            SetupRepositoryContains(true, _stoneDefinition.ItemID);
            SetupRepositoryGet(_stoneDefinition, _stoneDefinition.ItemID);

            Item item = _itemCreationService.Create(_stoneDefinition.ItemID, amount);

            AssertItem(item, _stoneDefinition, amount);
            VerifyRepositoryCalled(_stoneDefinition.ItemID);
        }

        [TestCase(0u)]
        [TestCase(1u)]
        [TestCase(uint.MaxValue)]
        public void Positive_Create_DifferentSellPrices_CreatesItem(uint sellPrice)
        {
            ItemDefinition stone = _stoneDefinition with { BaseSellPrice = sellPrice };

            SetupRepositoryContains(true, stone.ItemID);
            SetupRepositoryGet(stone, stone.ItemID);

            Item item = _itemCreationService.Create(stone.ItemID, 5);
            
            AssertItem(item, stone, 5);
            VerifyRepositoryCalled(_stoneDefinition.ItemID);
        }

        [Test]
        public void Negative_Create_DefinitionNotFound_Throws()
        {
            SetupRepositoryContains(false, _stoneDefinition.ItemID);
            
            Assert.Throws<NotFoundException<ItemID>>(() => _itemCreationService.Create(_stoneDefinition.ItemID, 1));

            _repositoryMock.Verify(library => library.Contains(_stoneDefinition.ItemID), Times.Once);
            VerifyRepositoryNoOtherCalls();
        }
    }
}