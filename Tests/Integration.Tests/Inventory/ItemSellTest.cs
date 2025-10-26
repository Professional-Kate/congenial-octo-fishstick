using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Currency.Contracts.Command;
using IdelPog.Inventory.Contracts.Command;
using IdelPog.Inventory.Contracts.Error;
using IdelPog.Inventory.Contracts.Response;
using IdelPog.Inventory.Exceptions;

namespace IdelPog.Integration.Tests.Inventory
{
    [TestFixture]
    public sealed class ItemSellTest : ManagedTestBuffer
    {
        private ManagedResponseListener<ItemSellResponse> _itemSellResponseListener;
        private ManagedErrorListener<ItemSellError> _itemSellErrorListener;
        private ManagedResponseListener<InventoryUpdateResponse> _inventoryUpdateResponseListener;
        private ManagedResponseListener<CurrencyUpdate> _currencyUpdateListener;
        
        private ItemSell _sellHerbs; 
        private InventoryUpdate _addHerbsUpdate;
        private ItemDefinitionCreation _herbsDefinition;
        private CurrencyCreation _goldCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _sellHerbs = new ItemSell
            {
                CurrencyType = CurrencyType.GOLD,
                ItemID = ItemID.HERBS,
                Amount = 1
            };

            _addHerbsUpdate = new InventoryUpdate
            {
                ItemID = ItemID.HERBS,
                ActionType = ActionType.ADD,
                Amount = 1
            };

            _herbsDefinition = new ItemDefinitionCreation
            {
                ItemID = ItemID.HERBS,
                BaseSellPrice = 3,
                Information = new Information { Name = "Herbs", Description = "Some are quite tasty!!" }
            };

            _goldCreation = new CurrencyCreation
            {
                CurrencyType = CurrencyType.GOLD, 
                StartingAmount = 0
            };
        }

        [SetUp]
        public void Setup()
        {
            _itemSellResponseListener = new ManagedResponseListener<ItemSellResponse>();
            _itemSellErrorListener = new ManagedErrorListener<ItemSellError>();
            _inventoryUpdateResponseListener = new ManagedResponseListener<InventoryUpdateResponse>();
            _currencyUpdateListener = new ManagedResponseListener<CurrencyUpdate>();
            
            ManagedSubscribe(_itemSellResponseListener);
            ManagedSubscribe(_itemSellErrorListener);
            ManagedSubscribe(_inventoryUpdateResponseListener);
            ManagedSubscribe(_currencyUpdateListener);
        } 
        
        private void DispatchCurrencyCreations(params CurrencyCreation[] currencyCreations)
        {
            IBuffer<CurrencyCreation> buffer = BufferManager.RequestBuffer<CurrencyCreation>(new BufferRequest(currencyCreations.Length));
            buffer.Assign(currencyCreations);
            buffer.MarkReady();
        }
        
        private void DispatchItemDefinitionCreations(params ItemDefinitionCreation[] creations)
        {
            IBuffer<ItemDefinitionCreation> buffer = BufferManager.RequestBuffer<ItemDefinitionCreation>(new BufferRequest(creations.Length));
            buffer.Assign(creations);
            buffer.MarkReady();
        }
        
        private void DispatchInventoryUpdate(params InventoryUpdate[] inventoryUpdates)
        {
            IBuffer<InventoryUpdate> buffer = BufferManager.RequestBuffer<InventoryUpdate>(new BufferRequest(inventoryUpdates.Length));
            buffer.Assign(inventoryUpdates);
            buffer.MarkReady();
        }

        private void DispatchItemSell(params ItemSell[] itemSells)
        {
            IBuffer<ItemSell> buffer = BufferManager.RequestBuffer<ItemSell>(new BufferRequest(itemSells.Length));
            buffer.Assign(itemSells);
            buffer.MarkReady();
        }

        private void AssertItemSellResponseCalled(bool wasCalled)
        {
            Assert.That(_itemSellResponseListener.WasCalled, Is.EqualTo(wasCalled));
        }

        private void AssertItemSellResponseLength(int length)
        {
            Assert.That(_itemSellResponseListener.Responses, Has.Length.EqualTo(length));
        }

        private static void AssertItemSellResponse(ItemSellResponse response, ItemSell itemSell)
        {
            Assert.Multiple(() =>
            {
                Assert.That(response.CurrencyType, Is.EqualTo(itemSell.CurrencyType));
                Assert.That(response.Amount, Is.EqualTo(itemSell.Amount));
                Assert.That(response.ItemID, Is.EqualTo(itemSell.ItemID));
            });
        }

        private void AssertItemSellErrorCalled(bool wasCalled)
        {
            Assert.That(_itemSellErrorListener.WasCalled, Is.EqualTo(wasCalled));
        }
        
        private void AssertItemSellErrorLength(int length)
        {
            Assert.That(_itemSellErrorListener.Error.ItemSells, Has.Length.EqualTo(length));
        }

        private void AssertItemSellError(Type exception, params ItemSell[] itemSells)
        {
            BaseError baseError = _itemSellErrorListener.Error.BaseError;
            Assert.Multiple(() =>
            {
                Assert.That(baseError.Exception, Is.TypeOf<ControllerThrownException>());
                Assert.That(baseError.Exception.InnerException, Is.TypeOf(exception));
                Assert.That(_itemSellErrorListener.Error.ItemSells, Is.EqualTo(itemSells));
            });
        }

        private void AssertInventoryUpdateResponseCalled(bool wasCalled)
        {
            Assert.That(_inventoryUpdateResponseListener.WasCalled, Is.EqualTo(wasCalled));
        }
        
        private void AssertInventoryUpdateResponseLength(int length)
        {
            Assert.That(_inventoryUpdateResponseListener.Responses, Has.Length.EqualTo(length));
        }

        private static void AssertInventoryUpdateResponse(InventoryUpdateResponse response, InventoryUpdate inventoryUpdate, MutateType mutateType)
        {
            ItemInfo itemInfo = response.ItemInfo;            
            Assert.Multiple(() =>
            {
                Assert.That(response.MutateType, Is.EqualTo(mutateType));
                Assert.That(itemInfo.ItemID, Is.EqualTo(inventoryUpdate.ItemID));
                Assert.That(itemInfo.Amount, Is.EqualTo(inventoryUpdate.Amount));
            });
        }
        
        private void AssertCurrencyUpdateCalled(bool wasCalled)
        { 
            Assert.That(_currencyUpdateListener.WasCalled, Is.EqualTo(wasCalled));
        }

        private void AssertCurrencyUpdateLength(int length)
        {
            Assert.That(_currencyUpdateListener.Responses, Has.Length.EqualTo(length));
        }

        private static void AssertCurrencyUpdate(CurrencyUpdate currencyUpdate, CurrencyType currencyType, uint amount)
        {
            Assert.Multiple(() =>
            {
                Assert.That(currencyUpdate.Amount, Is.EqualTo(amount));
                Assert.That(currencyUpdate.CurrencyType, Is.EqualTo(currencyType));
                Assert.That(currencyUpdate.ActionType, Is.EqualTo(ActionType.ADD));
            });
        }
        
        [Test]
        public void Positive_DispatchSingleCommand_DispatchesResponse_WithCurrencyUpdate()
        {
            DispatchCurrencyCreations(_goldCreation);
            DispatchItemDefinitionCreations(_herbsDefinition);
            DispatchInventoryUpdate(_addHerbsUpdate);
            
            Assert.DoesNotThrow(() => DispatchItemSell(_sellHerbs));
            
            AssertItemSellResponseCalled(true);
            AssertItemSellErrorCalled(false);
            AssertItemSellResponseLength(1);
            AssertItemSellResponse(_itemSellResponseListener.Responses[0], _sellHerbs);
            
            AssertInventoryUpdateResponseCalled(true);
            AssertInventoryUpdateResponseLength(1);
            AssertInventoryUpdateResponse(_inventoryUpdateResponseListener.Responses[0], _addHerbsUpdate with { Amount = 0 }, MutateType.DELETED);
            
            AssertCurrencyUpdateCalled(true);
            AssertCurrencyUpdateLength(1);
            AssertCurrencyUpdate(_currencyUpdateListener.Responses[0], _goldCreation.CurrencyType, _herbsDefinition.BaseSellPrice);
        }

        [Test]
        public void Positive_DispatchMultipleCommands_MultipleCurrency_DispatchesEverything()
        {
            CurrencyCreation gemsCreation = _goldCreation with { CurrencyType = CurrencyType.GEMS };
            
            DispatchCurrencyCreations(_goldCreation, gemsCreation);
            DispatchItemDefinitionCreations(_herbsDefinition);
            DispatchInventoryUpdate(_addHerbsUpdate with { Amount = 10 });
            
            Assert.DoesNotThrow(() => DispatchItemSell(_sellHerbs with { Amount = 3 }, _sellHerbs with { CurrencyType = CurrencyType.GEMS }));
            
            AssertItemSellResponseCalled(true);
            AssertItemSellErrorCalled(false);
            AssertItemSellResponseLength(2);
            AssertItemSellResponse(_itemSellResponseListener.Responses[0], _sellHerbs with { Amount = 3 });
            AssertItemSellResponse(_itemSellResponseListener.Responses[1], _sellHerbs with { CurrencyType = CurrencyType.GEMS });
            
            AssertInventoryUpdateResponseCalled(true);
            AssertInventoryUpdateResponseLength(1);
            AssertInventoryUpdateResponse(_inventoryUpdateResponseListener.Responses[0], _addHerbsUpdate with { Amount = 6 }, MutateType.CHANGED);
            
            AssertCurrencyUpdateCalled(true);
            AssertCurrencyUpdateLength(2);
            AssertCurrencyUpdate(_currencyUpdateListener.Responses[0], _goldCreation.CurrencyType, _herbsDefinition.BaseSellPrice * 3);
            AssertCurrencyUpdate(_currencyUpdateListener.Responses[1], gemsCreation.CurrencyType, _herbsDefinition.BaseSellPrice);
        }

        [Test]
        public void Positive_DispatchMultipleCommands_MultipleItems_SellsEach()
        {
            ItemSell emeraldSell = _sellHerbs with { ItemID = ItemID.EMERALD };
            ItemDefinitionCreation emeraldDefinition = _herbsDefinition with { ItemID = ItemID.EMERALD };
            InventoryUpdate addEmeraldUpdate = _addHerbsUpdate with { ItemID = ItemID.EMERALD };
            
            DispatchCurrencyCreations(_goldCreation);
            DispatchItemDefinitionCreations(_herbsDefinition, emeraldDefinition);
            DispatchInventoryUpdate(_addHerbsUpdate, addEmeraldUpdate);
            
            Assert.DoesNotThrow(() => DispatchItemSell(_sellHerbs, emeraldSell));
            
            AssertItemSellResponseCalled(true);
            AssertItemSellErrorCalled(false);
            AssertItemSellResponseLength(2);
            
            AssertInventoryUpdateResponseCalled(true);
            AssertInventoryUpdateResponseLength(2);
            AssertInventoryUpdateResponse(_inventoryUpdateResponseListener.Responses[0], _addHerbsUpdate with { Amount = 0 }, MutateType.DELETED);
            AssertInventoryUpdateResponse(_inventoryUpdateResponseListener.Responses[1], addEmeraldUpdate with { Amount = 0 }, MutateType.DELETED);
            
            AssertCurrencyUpdateCalled(true);
            AssertCurrencyUpdateLength(2);
        }

        [Test]
        public void Negative_SingleCommand_ItemNotFound_DispatchesError()
        {
            DispatchCurrencyCreations(_goldCreation);
            DispatchItemDefinitionCreations(_herbsDefinition);
            
            Assert.DoesNotThrow(() => DispatchItemSell(_sellHerbs));
            
            AssertItemSellResponseCalled(false);
            AssertItemSellErrorCalled(true);
            AssertItemSellErrorLength(1);
            AssertItemSellError(typeof(NotFoundException<ItemID>), _sellHerbs);
            
            AssertInventoryUpdateResponseCalled(false);
            AssertCurrencyUpdateCalled(false);
        }

        [Test]
        public void Negative_SingleCommand_NotEnoughItem_DispatchesError()
        {
            ItemSell notEnoughItemSell = _sellHerbs with { Amount = 10 };
            
            DispatchCurrencyCreations(_goldCreation);
            DispatchItemDefinitionCreations(_herbsDefinition);
            DispatchInventoryUpdate(_addHerbsUpdate);
            
            Assert.DoesNotThrow(() => DispatchItemSell(notEnoughItemSell));
            
            AssertItemSellResponseCalled(false);
            AssertItemSellErrorCalled(true);
            AssertItemSellErrorLength(1);
            AssertItemSellError(typeof(InsufficientAmountException), notEnoughItemSell);
            
            AssertCurrencyUpdateCalled(false);
        }

        [Test]
        public void Negative_SingleCommand_ZeroAmount_DispatchesError()
        {
            ItemSell zeroAmountSell = _sellHerbs with { Amount = 0 };
            
            DispatchCurrencyCreations(_goldCreation);
            DispatchItemDefinitionCreations(_herbsDefinition);
            
            Assert.DoesNotThrow(() => DispatchItemSell(zeroAmountSell));
            
            AssertItemSellResponseCalled(false);
            AssertItemSellErrorCalled(true);
            AssertItemSellErrorLength(1);
            AssertItemSellError(typeof(AmountZeroException), zeroAmountSell);
            
            AssertInventoryUpdateResponseCalled(false);
            AssertCurrencyUpdateCalled(false);
        }

        [Test]
        public void Negative_SingleCommand_ItemDefinitionNotFound_DispatchesError()
        {
            DispatchCurrencyCreations(_goldCreation);
            
            Assert.DoesNotThrow(() => DispatchItemSell(_sellHerbs));
            
            AssertItemSellResponseCalled(false);
            AssertItemSellErrorCalled(true);
            AssertItemSellErrorLength(1);
            AssertItemSellError(typeof(NotFoundException<ItemID>), _sellHerbs);
            
            AssertInventoryUpdateResponseCalled(false);
            AssertCurrencyUpdateCalled(false);
        }
    }
}