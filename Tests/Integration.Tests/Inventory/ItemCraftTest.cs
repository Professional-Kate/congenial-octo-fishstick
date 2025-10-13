using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Contracts.Command;
using IdelPog.Inventory.Contracts.Error;
using IdelPog.Inventory.Contracts.Response;
using IdelPog.Inventory.Exceptions;

namespace IdelPog.Integration.Tests.Inventory
{
    [TestFixture]
    public sealed class ItemCraftTest : ManagedTestBuffer
    {
        private ItemCraft _ringCraft;
        private RecipeCreation _ringCreation;
        private InventoryUpdate _ironUpdate;
        private InventoryUpdate _ringUpdate;
        
        private ManagedResponseListener<ItemCraftResponse> _itemCraftResponseListener;
        private ManagedErrorListener<ItemCraftError> _itemCraftErrorListener;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _ringCreation = new RecipeCreation
            {
                RecipeID = RecipeID.IRON_RING,
                RecipeInputs = [new RecipeInput { ItemID = ItemID.IRON, Amount = 1 }],
                RecipeOutputs = [new  RecipeOutput { ItemID = ItemID.RING, Amount = 1 }]
            };
            
            _ringCraft = new ItemCraft { RecipeID = RecipeID.IRON_RING,  Amount = 1 };

            _ironUpdate = new InventoryUpdate { ItemID = ItemID.IRON, Amount = 1, ActionType = ActionType.ADD };
            _ringUpdate = new InventoryUpdate { ItemID = ItemID.RING, Amount = 1, ActionType = ActionType.ADD };
        }

        [SetUp]
        public void Setup()
        {
            _itemCraftResponseListener = new  ManagedResponseListener<ItemCraftResponse>();
            _itemCraftErrorListener = new  ManagedErrorListener<ItemCraftError>();
            
            ManagedSubscribe(_itemCraftResponseListener);
            ManagedSubscribe(_itemCraftErrorListener);
        }

        private ManagedResponseListener<InventoryUpdateResponse> SubscribeInventoryUpdateResponseListener()
        {
            ManagedResponseListener<InventoryUpdateResponse> inventoryUpdateResponseListener = new();
            ManagedSubscribe(inventoryUpdateResponseListener);

            return inventoryUpdateResponseListener;
        }
        
        private void DispatchInventoryUpdates(params InventoryUpdate[] inventoryUpdates)
        {
            IBuffer<InventoryUpdate> buffer = BufferManager.RequestBuffer<InventoryUpdate>(new BufferRequest(inventoryUpdates.Length));
            buffer.Assign(inventoryUpdates);
            buffer.MarkReady();
        }
        
        private void DispatchRecipeCreations(params RecipeCreation[] recipeCreations)
        {
            IBuffer<RecipeCreation> buffer = BufferManager.RequestBuffer<RecipeCreation>(new BufferRequest(recipeCreations.Length));
            buffer.Assign(recipeCreations);
            buffer.MarkReady();
        }

        private void DispatchItemCrafts(params ItemCraft[] itemCrafts)
        {
            IBuffer<ItemCraft> buffer = BufferManager.RequestBuffer<ItemCraft>(new BufferRequest(itemCrafts.Length));
            buffer.Assign(itemCrafts);
            buffer.MarkReady();
        }

        private void AssertCraftResponseListenerCalled(bool called)
        {
            Assert.That(_itemCraftResponseListener.WasCalled, Is.EqualTo(called));
        }

        private void AssertCraftListenerLength(int length)
        {
            Assert.That(_itemCraftResponseListener.Responses, Has.Length.EqualTo(length));
        }

        private static void AssertCraftResponse(ItemCraftResponse response, ItemCraft itemCraft)
        {
            Assert.That(response.RecipeID, Is.EqualTo(itemCraft.RecipeID));
        }

        private static void AssertInventoryUpdateResponseListenerCalled(ManagedResponseListener<InventoryUpdateResponse> listener, bool called)
        {
            Assert.That(listener.WasCalled, Is.EqualTo(called));
        }

        private static void AssertInventoryUpdateResponseLength(ManagedResponseListener<InventoryUpdateResponse> listener, int length)
        {
            Assert.That(listener.Responses, Has.Length.EqualTo(length));
        }

        private static void AssertInventoryResponse(InventoryUpdateResponse response, InventoryUpdate update, MutateType mutateType)
        {
            Assert.Multiple(() =>
            {
                Assert.That(response.ItemInfo.ItemID, Is.EqualTo(update.ItemID));
                Assert.That(response.ItemInfo.Amount, Is.EqualTo(update.Amount));
                Assert.That(response.MutateType, Is.EqualTo(mutateType));
            });
        }

        private void AssertErrorListenerCalled(bool called)
        {
            Assert.That(_itemCraftErrorListener.WasCalled, Is.EqualTo(called));
        }

        private void AssertErrorLength(int length)
        {
            Assert.That(_itemCraftErrorListener.Error.ItemCrafts,  Has.Length.EqualTo(length));
        }

        private void AssertError(Type exception, params ItemCraft[] itemCrafts)
        {
            ItemCraftError error = _itemCraftErrorListener.Error;
            Assert.Multiple(() =>
            {
                Assert.That(error.ItemCrafts, Is.EqualTo(itemCrafts));
                Assert.That(error.BaseError.Exception, Is.TypeOf<ControllerThrownException>());
                Assert.That(error.BaseError.Exception.InnerException, Is.TypeOf(exception));
            });
        }

        [Test] 
        public void Positive_DispatchCraft_CraftsOneItem_DispatchesResponses()
        {
            DispatchInventoryUpdates(_ironUpdate);
            DispatchRecipeCreations(_ringCreation);
            ManagedResponseListener<InventoryUpdateResponse> listener = SubscribeInventoryUpdateResponseListener();
            
            Assert.DoesNotThrow(() => DispatchItemCrafts(_ringCraft));

            AssertCraftResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertCraftListenerLength(1);
            AssertCraftResponse(_itemCraftResponseListener.Responses[0], _ringCraft);
            
            AssertInventoryUpdateResponseListenerCalled(listener, true);
            AssertInventoryUpdateResponseLength(listener, 2);
            AssertInventoryResponse(listener.Responses[0], _ironUpdate with { ActionType = ActionType.REMOVE, Amount = 0 }, MutateType.DELETED);
            AssertInventoryResponse(listener.Responses[1], _ringUpdate, MutateType.CREATED);
        }

        [Test]
        public void Positive_DispatchCraft_CraftsMultipleItems_DispatchesResponses()
        {
            ItemCraft twoRingsCraft = _ringCraft with { Amount = 2 };
            
            DispatchInventoryUpdates(_ironUpdate with { Amount = 10 });
            DispatchRecipeCreations(_ringCreation);
            ManagedResponseListener<InventoryUpdateResponse> listener = SubscribeInventoryUpdateResponseListener();
            
            Assert.DoesNotThrow(() => DispatchItemCrafts(twoRingsCraft));

            AssertCraftResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertCraftListenerLength(1);
            AssertCraftResponse(_itemCraftResponseListener.Responses[0], twoRingsCraft);
            
            AssertInventoryUpdateResponseListenerCalled(listener, true);
            AssertInventoryUpdateResponseLength(listener, 2);
            AssertInventoryResponse(listener.Responses[0], _ironUpdate with { ActionType = ActionType.REMOVE, Amount = 8 }, MutateType.CHANGED);
            AssertInventoryResponse(listener.Responses[1], _ringUpdate with { Amount = 2 }, MutateType.CREATED);
        }

        [Test]
        public void Positive_DispatchCraft_MultipleMessages_DispatchesResponses()
        {
            DispatchInventoryUpdates(_ironUpdate with { Amount = 10 });
            DispatchRecipeCreations(_ringCreation);
            ManagedResponseListener<InventoryUpdateResponse> listener = SubscribeInventoryUpdateResponseListener();
            
            Assert.DoesNotThrow(() => DispatchItemCrafts(_ringCraft, _ringCraft));

            AssertCraftResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertCraftListenerLength(2);
            AssertCraftResponse(_itemCraftResponseListener.Responses[0], _ringCraft);
            AssertCraftResponse(_itemCraftResponseListener.Responses[1], _ringCraft);
            
            AssertInventoryUpdateResponseListenerCalled(listener, true);
            AssertInventoryUpdateResponseLength(listener, 2);
            AssertInventoryResponse(listener.Responses[0], _ironUpdate with { ActionType = ActionType.REMOVE, Amount = 8 }, MutateType.CHANGED);
            AssertInventoryResponse(listener.Responses[1], _ringUpdate with { Amount = 2 }, MutateType.CREATED);
        }

        [Test]
        public void Positive_DispatchCraft_MultipleInputsAndOutputs_DispatchesResponses()
        {
            RecipeCreation creation = new()
            {
                RecipeID = RecipeID.IRON_RING,
                RecipeInputs = [new RecipeInput { ItemID = ItemID.IRON, Amount = 1 }, new RecipeInput { ItemID = ItemID.DIAMOND, Amount = 1 }],
                RecipeOutputs = [new  RecipeOutput { ItemID = ItemID.RING, Amount = 1 }, new  RecipeOutput { ItemID = ItemID.SAND, Amount = 1 }]
            };
            
            DispatchInventoryUpdates(_ironUpdate, _ironUpdate with { ItemID = ItemID.DIAMOND});
            DispatchRecipeCreations(creation);
            ManagedResponseListener<InventoryUpdateResponse> listener = SubscribeInventoryUpdateResponseListener();
            
            Assert.DoesNotThrow(() => DispatchItemCrafts(_ringCraft));

            AssertCraftResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertCraftListenerLength(1);
            AssertCraftResponse(_itemCraftResponseListener.Responses[0], _ringCraft);
            
            AssertInventoryUpdateResponseListenerCalled(listener, true);
            AssertInventoryUpdateResponseLength(listener, 4);
            AssertInventoryResponse(listener.Responses[0], _ironUpdate with { ActionType = ActionType.REMOVE, Amount = 0 }, MutateType.DELETED);
            AssertInventoryResponse(listener.Responses[1], _ringUpdate with { ItemID = ItemID.DIAMOND, Amount = 0}, MutateType.DELETED);
            AssertInventoryResponse(listener.Responses[2], _ringUpdate, MutateType.CREATED);
            AssertInventoryResponse(listener.Responses[3], _ringUpdate with { ItemID = ItemID.SAND }, MutateType.CREATED);
        }

        [Test]
        public void Negative_DispatchCraft_CraftNotFound_DispatchesError()
        {
            ManagedResponseListener<InventoryUpdateResponse> listener = SubscribeInventoryUpdateResponseListener();
            
            Assert.DoesNotThrow(() => DispatchItemCrafts(_ringCraft));

            AssertCraftResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError(typeof(NotFoundException<RecipeID>), _ringCraft);
            
            AssertInventoryUpdateResponseListenerCalled(listener, false);
        }

        [Test]
        public void Negative_DispatchCraft_NoItemInInventory_DispatchesError()
        {
            DispatchRecipeCreations(_ringCreation);
            ManagedResponseListener<InventoryUpdateResponse> listener = SubscribeInventoryUpdateResponseListener();
            
            Assert.DoesNotThrow(() => DispatchItemCrafts(_ringCraft));

            AssertCraftResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError(typeof(NotFoundException<ItemID>), _ringCraft);
            
            AssertInventoryUpdateResponseListenerCalled(listener, false);
        }
        
        [Test]
        public void Negative_DispatchCraft_ItemNotEnoughAmount_DispatchesError()
        {
            DispatchInventoryUpdates(_ironUpdate);
            DispatchRecipeCreations(_ringCreation);
            ManagedResponseListener<InventoryUpdateResponse> listener = SubscribeInventoryUpdateResponseListener();
            
            Assert.DoesNotThrow(() => DispatchItemCrafts(_ringCraft with { Amount = 2 }));

            AssertCraftResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError(typeof(InsufficientAmountException), _ringCraft with { Amount = 2 });
            
            AssertInventoryUpdateResponseListenerCalled(listener, false);
        }

        [Test]
        public void Negative_DispatchCraft_ZeroAmount_DispatchesError()
        {
            DispatchInventoryUpdates(_ironUpdate);
            DispatchRecipeCreations(_ringCreation);
            ManagedResponseListener<InventoryUpdateResponse> listener = SubscribeInventoryUpdateResponseListener();
            
            Assert.DoesNotThrow(() => DispatchItemCrafts(_ringCraft with { Amount = 0 }));

            AssertCraftResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError(typeof(AmountZeroException), _ringCraft with { Amount = 0 });
            
            AssertInventoryUpdateResponseListenerCalled(listener, false);
        }
    }
}