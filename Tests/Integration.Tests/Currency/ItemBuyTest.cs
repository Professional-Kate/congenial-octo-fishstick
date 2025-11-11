using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Currency.Contracts.Command;
using IdelPog.Currency.Contracts.Error;
using IdelPog.Currency.Contracts.Response;
using IdelPog.Currency.Exceptions;
using IdelPog.Inventory.Contracts.Command;

namespace IdelPog.Integration.Tests.Currency
{
    [TestFixture]
    public sealed class ItemBuyTest : ManagedTestBuffer
    {
        private ManagedResponseListener<ItemBuyResponse> _itemBuyResponseListener;
        private ManagedErrorListener<ItemBuyError> _itemBuyErrorListener;
        private ManagedResponseListener<CurrencyUpdateResponse> _currencyUpdateResponseListener;
        private ManagedResponseListener<InventoryUpdate> _inventoryUpdateListener;

        private ItemBuy _rubyBuy;
        private CurrencyCreation _goldCreation;
        private ItemDefinitionCreation _rubyDefinition;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _rubyBuy = new ItemBuy
            {
                CurrencyType = CurrencyType.GOLD,
                ItemID = ItemID.RUBY,
                Price = 5,
                Amount = 1
            };

            _rubyDefinition = new ItemDefinitionCreation
            {
                ItemID = ItemID.RUBY,
                BaseSellPrice = 10,
                Information = new Information { Name = "Ruby ruby...", Description = "Ruby!" }
            };
            
            _goldCreation = new CurrencyCreation { CurrencyType = CurrencyType.GOLD, StartingAmount = 100 };
        }

        [SetUp]
        public void Setup()
        {
            _itemBuyResponseListener = new ManagedResponseListener<ItemBuyResponse>();
            _itemBuyErrorListener = new ManagedErrorListener<ItemBuyError>();
            _currencyUpdateResponseListener = new ManagedResponseListener<CurrencyUpdateResponse>();
            _inventoryUpdateListener = new ManagedResponseListener<InventoryUpdate>();
            
            ManagedSubscribe(_itemBuyResponseListener);
            ManagedSubscribe(_itemBuyErrorListener);
            ManagedSubscribe(_currencyUpdateResponseListener);
            ManagedSubscribe(_inventoryUpdateListener);
        }
        
        private void DispatchItemDefinitionCreations(params ItemDefinitionCreation[] itemDefinitionCreations)
        {
            IBuffer<ItemDefinitionCreation> buffer = BufferManager.RequestBuffer<ItemDefinitionCreation>(new BufferRequest(itemDefinitionCreations.Length));
            buffer.Assign(itemDefinitionCreations);
            buffer.MarkReady();
        }
        
        private void DispatchCurrencyCreations(params CurrencyCreation[] currencyCreations)
        {
            IBuffer<CurrencyCreation> buffer = BufferManager.RequestBuffer<CurrencyCreation>(new BufferRequest(currencyCreations.Length));
            buffer.Assign(currencyCreations);
            buffer.MarkReady();
        }

        private void DispatchItemBuys(params ItemBuy[] itemBuys)
        {
            IBuffer<ItemBuy> buffer = BufferManager.RequestBuffer<ItemBuy>(new BufferRequest(itemBuys.Length));
            buffer.Assign(itemBuys);
            buffer.MarkReady();
        }

        private void AssertItemBuyResponseListenerCalled(bool wasCalled)
        { 
            Assert.That(_itemBuyResponseListener.WasCalled, Is.EqualTo(wasCalled));
        }

        private void AssertItemBuyResponseLength(int length)
        { 
            Assert.That(_itemBuyResponseListener.Responses, Has.Length.EqualTo(length));
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
        
        private void AssertItemBuyErrorListenerCalled(bool wasCalled)
        { 
            Assert.That(_itemBuyErrorListener.WasCalled, Is.EqualTo(wasCalled));
        }

        private void AssertItemBuyErrorLength(int length)
        { 
            Assert.That(_itemBuyErrorListener.Error.ItemBuys, Has.Length.EqualTo(length));
        }

        private void AssertItemBuyError<TException>(params ItemBuy[] itemBuys)
        {
            BaseError baseError = _itemBuyErrorListener.Error.BaseError;
            Assert.Multiple(() =>
            {
                Assert.That(_itemBuyErrorListener.Error.ItemBuys, Is.EquivalentTo(itemBuys));
                Assert.That(baseError.Exception, Is.TypeOf<ControllerThrownException>());
                Assert.That(baseError.Exception.InnerException, Is.TypeOf<TException>());
            });
        }

        private void AssertCurrencyUpdateResponseListenerCalled(bool wasCalled)
        {
            Assert.That(_currencyUpdateResponseListener.WasCalled, Is.EqualTo(wasCalled));
        }

        private void AssertCurrencyUpdateResponseLength(int length)
        {
            Assert.That(_currencyUpdateResponseListener.Responses, Has.Length.EqualTo(length));
        }

        private static void AssertCurrencyUpdateResponse(CurrencyUpdateResponse currencyUpdateResponse, CurrencyType currencyType, uint amount)
        {
            Assert.Multiple(() =>
            {
                Assert.That(currencyUpdateResponse.CurrencyType, Is.EqualTo(currencyType));
                Assert.That(currencyUpdateResponse.CurrencyAmount, Is.EqualTo(amount));
            });
        }

        private void AssertInventoryUpdateListenerCalled(bool wasCalled)
        {
            Assert.That(_inventoryUpdateListener.WasCalled, Is.EqualTo(wasCalled));
        }

        private void AssertInventoryUpdateLength(int length)
        {
            Assert.That(_inventoryUpdateListener.Responses, Has.Length.EqualTo(length));
        }

        private static void AssertInventoryUpdate(InventoryUpdate inventoryUpdate, ItemID itemID, uint amount)
        {
            Assert.Multiple(() =>
            {
                Assert.That(inventoryUpdate.ItemID, Is.EqualTo(itemID));
                Assert.That(inventoryUpdate.Amount, Is.EqualTo(amount));
                Assert.That(inventoryUpdate.ActionType, Is.EqualTo(ActionType.ADD));
            });
        }

        [Test]
        public void Positive_SendRubyBuy_RemovesGold_AddsRuby()
        {
            DispatchCurrencyCreations(_goldCreation);
            DispatchItemDefinitionCreations(_rubyDefinition);
            
            Assert.DoesNotThrow(() => DispatchItemBuys(_rubyBuy));

            AssertItemBuyResponseListenerCalled(true);
            AssertItemBuyErrorListenerCalled(false);
            AssertItemBuyResponseLength(1);
            AssertItemBuyResponse(_itemBuyResponseListener.Responses[0], _rubyBuy);

            AssertCurrencyUpdateResponseListenerCalled(true);
            AssertCurrencyUpdateResponseLength(1);
            AssertCurrencyUpdateResponse(_currencyUpdateResponseListener.Responses[0], _goldCreation.CurrencyType, _goldCreation.StartingAmount - _rubyBuy.Price);

            AssertInventoryUpdateListenerCalled(true);
            AssertInventoryUpdateLength(1);
            AssertInventoryUpdate(_inventoryUpdateListener.Responses[0], _rubyBuy.ItemID, _rubyBuy.Amount);
        }

        [Test]
        public void Positive_SendMultipleCommands_DispatchesCorrectResponses()
        {
            DispatchCurrencyCreations(_goldCreation);
            DispatchItemDefinitionCreations(_rubyDefinition);
            
            Assert.DoesNotThrow(() => DispatchItemBuys(_rubyBuy, _rubyBuy));

            AssertItemBuyResponseListenerCalled(true);
            AssertItemBuyErrorListenerCalled(false);
            AssertItemBuyResponseLength(2);
            AssertItemBuyResponse(_itemBuyResponseListener.Responses[0], _rubyBuy);
            AssertItemBuyResponse(_itemBuyResponseListener.Responses[1], _rubyBuy);

            AssertCurrencyUpdateResponseListenerCalled(true);
            AssertCurrencyUpdateResponseLength(1);
            AssertCurrencyUpdateResponse(_currencyUpdateResponseListener.Responses[0], _goldCreation.CurrencyType, _goldCreation.StartingAmount - _rubyBuy.Price * 2);

            AssertInventoryUpdateListenerCalled(true);
            AssertInventoryUpdateLength(2);
            AssertInventoryUpdate(_inventoryUpdateListener.Responses[0], _rubyBuy.ItemID, _rubyBuy.Amount);
            AssertInventoryUpdate(_inventoryUpdateListener.Responses[1], _rubyBuy.ItemID, _rubyBuy.Amount);
        }

        [Test]
        public void Positive_SendRubyBuy_NoItemDefinitionCreated_OperationCompletes()
        {
            // A missing ItemDefinition (From ItemDefinitionCreation) will not cause the operation to fail. This is a data setup issue.
            DispatchCurrencyCreations(_goldCreation);
            
            Assert.DoesNotThrow(() => DispatchItemBuys(_rubyBuy));

            AssertItemBuyResponseListenerCalled(true);
            AssertItemBuyErrorListenerCalled(false);
            AssertItemBuyResponseLength(1);
            AssertItemBuyResponse(_itemBuyResponseListener.Responses[0], _rubyBuy);

            AssertCurrencyUpdateResponseListenerCalled(true);
            AssertCurrencyUpdateResponseLength(1);
            AssertCurrencyUpdateResponse(_currencyUpdateResponseListener.Responses[0], _goldCreation.CurrencyType, _goldCreation.StartingAmount - _rubyBuy.Price);

            // This InventoryUpdate will fail but we don't care. 
            AssertInventoryUpdateListenerCalled(true);
            AssertInventoryUpdateLength(1);
            AssertInventoryUpdate(_inventoryUpdateListener.Responses[0], _rubyBuy.ItemID, _rubyBuy.Amount);
        }

        [Test]
        public void Negative_SendRubyBuy_CurrencyNotFound_DispatchesError()
        {
            DispatchItemDefinitionCreations(_rubyDefinition);
            
            Assert.DoesNotThrow(() => DispatchItemBuys(_rubyBuy));
            
            AssertItemBuyResponseListenerCalled(false);
            AssertItemBuyErrorListenerCalled(true);
            AssertItemBuyErrorLength(1);
            AssertItemBuyError<NotFoundException<CurrencyType>>(_rubyBuy);

            AssertCurrencyUpdateResponseListenerCalled(false);
            AssertInventoryUpdateListenerCalled(false);
        }
        
        [Test]
        public void Negative_SendRubyBuy_NotEnoughCurrency_DispatchesError()
        {
            DispatchCurrencyCreations(_goldCreation with { StartingAmount = 0 });
            DispatchItemDefinitionCreations(_rubyDefinition);
            
            Assert.DoesNotThrow(() => DispatchItemBuys(_rubyBuy));
            
            AssertItemBuyResponseListenerCalled(false);
            AssertItemBuyErrorListenerCalled(true);
            AssertItemBuyErrorLength(1);
            AssertItemBuyError<NotEnoughCurrencyException>(_rubyBuy);

            AssertCurrencyUpdateResponseListenerCalled(false);
            AssertInventoryUpdateListenerCalled(false);
        }
        
        [Test]
        public void Negative_SendRubyBuy_ZeroPrice_DispatchesError()
        {
            ItemBuy zeroPriceBuy = _rubyBuy with { Price = 0 };
            
            DispatchCurrencyCreations(_goldCreation);
            DispatchItemDefinitionCreations(_rubyDefinition);
            
            Assert.DoesNotThrow(() => DispatchItemBuys(zeroPriceBuy));
            
            AssertItemBuyResponseListenerCalled(false);
            AssertItemBuyErrorListenerCalled(true);
            AssertItemBuyErrorLength(1);
            AssertItemBuyError<AmountZeroException>(zeroPriceBuy);

            AssertCurrencyUpdateResponseListenerCalled(false);
            AssertInventoryUpdateListenerCalled(false);
        }
        
        [Test]
        public void Negative_SendRubyBuy_ZeroAmount_DispatchesError()
        {
            ItemBuy zeroAmountBuy = _rubyBuy with { Amount = 0 };
            
            DispatchCurrencyCreations(_goldCreation);
            DispatchItemDefinitionCreations(_rubyDefinition);
            
            Assert.DoesNotThrow(() => DispatchItemBuys(zeroAmountBuy));
            
            AssertItemBuyResponseListenerCalled(false);
            AssertItemBuyErrorListenerCalled(true);
            AssertItemBuyErrorLength(1);
            AssertItemBuyError<AmountZeroException>(zeroAmountBuy);

            AssertCurrencyUpdateResponseListenerCalled(false);
            AssertInventoryUpdateListenerCalled(false);
        }
    }
}