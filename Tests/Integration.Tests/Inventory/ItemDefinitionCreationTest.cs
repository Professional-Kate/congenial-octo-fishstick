using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Inventory.Contracts.Command;
using IdelPog.Inventory.Contracts.Error;
using IdelPog.Inventory.Contracts.Response;
using IdelPog.Inventory.Exceptions;

namespace IdelPog.Integration.Tests.Inventory
{
    [TestFixture]
    public sealed class ItemDefinitionCreationTest : ManagedTestBuffer
    {
        private ItemDefinitionCreation _smallInsectsDefinition;

        private ManagedResponseListener<ItemDefinitionCreationResponse> _responseListener;
        private ManagedErrorListener<ItemDefinitionCreationError> _errorListener;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _smallInsectsDefinition = new ItemDefinitionCreation
            {
                ItemID = ItemID.SMALL_INSECTS,
                BaseSellPrice = 1,
                Information = new Information { Name = "Insects", Description = "Small" }
            };
        }

        [SetUp]
        public void Setup()
        {
            _responseListener = new ManagedResponseListener<ItemDefinitionCreationResponse>();
            _errorListener = new ManagedErrorListener<ItemDefinitionCreationError>();
            
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
        }
        
        private void DispatchItemDefinitionCreations(params ItemDefinitionCreation[] creations)
        {
            IBuffer<ItemDefinitionCreation> buffer = BufferManager.RequestBuffer<ItemDefinitionCreation>(new BufferRequest(creations.Length));
            buffer.Assign(creations);
            buffer.MarkReady();
        }

        private void AssertResponseListenerCalled(bool wasCalled)
        {
            Assert.That(_responseListener.WasCalled, Is.EqualTo(wasCalled));
        }

        private void AssertResponseLength(int length)
        {
            Assert.That(_responseListener.Responses, Has.Length.EqualTo(length));
        }

        private void AssertResponse(ItemDefinitionCreationResponse response, ItemDefinitionCreation creation)
        {
            Assert.Multiple(() =>
            {
                Assert.That(response.ItemID, Is.EqualTo(creation.ItemID));
                Assert.That(response.BaseSellPrice, Is.EqualTo(creation.BaseSellPrice));
                Assert.That(response.Information, Is.EqualTo(creation.Information));
            });
        } 
        
        private void AssertErrorListenerCalled(bool wasCalled)
        {
            Assert.That(_errorListener.WasCalled, Is.EqualTo(wasCalled));
        }

        private void AssertErrorLength(int length)
        {
            Assert.That(_errorListener.Error.ItemDefinitionCreations, Has.Length.EqualTo(length));
        }

        private void AssertError(Type exception, params ItemDefinitionCreation[] creations)
        {
            ItemDefinitionCreationError creationError = _errorListener.Error;
            BaseError baseError = creationError.BaseError;
            
            Assert.Multiple(() =>
            {
                Assert.That(baseError.Exception, Is.TypeOf<ControllerThrownException>());
                Assert.That(baseError.Exception.InnerException, Is.TypeOf(exception));
                Assert.That(creationError.ItemDefinitionCreations, Is.EqualTo(creations));
            });
        }

        [Test]
        public void Positive_SendCreationCommand_CreatesDefinition_DispatchesResponse()
        {
            Assert.DoesNotThrow(() => DispatchItemDefinitionCreations(_smallInsectsDefinition));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], _smallInsectsDefinition);
        }

        [Test]
        public void Positive_SendMultipleCommands_CreatesMultiple_DispatchesResponses()
        {
            ItemDefinitionCreation copperDefinition = new()
            {
                ItemID = ItemID.COPPER,
                BaseSellPrice = 3,
                Information = new Information { Name = "Copper", Description = "Tasty" }
            };
            
            Assert.DoesNotThrow(() => DispatchItemDefinitionCreations(_smallInsectsDefinition, copperDefinition));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(2);
            AssertResponse(_responseListener.Responses[0], _smallInsectsDefinition);
            AssertResponse(_responseListener.Responses[1], copperDefinition);
        }

        [Test]
        public void Negative_SendSingleCommand_ZeroSellPrice_DispatchesError()
        {
            ItemDefinitionCreation zeroSellPriceCreation = _smallInsectsDefinition with { BaseSellPrice = 0 };
            
            Assert.DoesNotThrow(() => DispatchItemDefinitionCreations(zeroSellPriceCreation));
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError(typeof(AmountZeroException), zeroSellPriceCreation);
        }

        [Test]
        public void Negative_SendMultipleCommands_DuplicateItemID_DispatchesError()
        {
            Assert.DoesNotThrow(() => DispatchItemDefinitionCreations(_smallInsectsDefinition, _smallInsectsDefinition));
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(2);
            AssertError(typeof(DuplicateEntityException), _smallInsectsDefinition, _smallInsectsDefinition);
        }
    }
}