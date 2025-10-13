using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Contracts.Command;
using IdelPog.Inventory.Contracts.Error;
using IdelPog.Inventory.Contracts.Response;

namespace IdelPog.Integration.Tests.Inventory
{
    [TestFixture]
    public sealed class RecipeCreationTest : ManagedTestBuffer
    {
        private RecipeCreation _ringCreation;
        private RecipeCreation _diamondRingCreation;
        
        private ManagedResponseListener<RecipeCreationResponse> _responseListener;
        private ManagedErrorListener<RecipeCreationError> _errorListener;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _ringCreation = new RecipeCreation
            {
                RecipeID = RecipeID.IRON_RING,
                RecipeInputs = [new RecipeInput { ItemID = ItemID.IRON, Amount = 1 }],
                RecipeOutputs = [new  RecipeOutput { ItemID = ItemID.RING, Amount = 1 }]
            };
            
            _diamondRingCreation = new RecipeCreation
            {
                RecipeID = RecipeID.DIAMOND_RING,
                RecipeInputs = [new RecipeInput { ItemID = ItemID.RING, Amount = 1 }, new RecipeInput { ItemID = ItemID.DIAMOND, Amount = 1 }],
                RecipeOutputs = [new  RecipeOutput { ItemID = ItemID.DIAMOND_RING, Amount = 1 }]
            };
        }

        [SetUp]
        public void Setup()
        {
            _responseListener = new ManagedResponseListener<RecipeCreationResponse>();
            _errorListener = new ManagedErrorListener<RecipeCreationError>();
            
            ManagedSubscribe(_responseListener);
            ManagedSubscribe(_errorListener);
        }

        private void DispatchRecipeCreations(params RecipeCreation[] recipeCreations)
        {
            IBuffer<RecipeCreation> buffer = BufferManager.RequestBuffer<RecipeCreation>(new BufferRequest(recipeCreations.Length));
            buffer.Assign(recipeCreations);
            buffer.MarkReady();
        }

        private void AssertResponseListenerCalled(bool called)
        {
            Assert.That(_responseListener.WasCalled, Is.EqualTo(called));
        }

        private void AssertResponseLength(int length)
        {
            Assert.That(_responseListener.Responses, Has.Length.EqualTo(length));
        }

        private static void AssertResponse(RecipeCreationResponse response, RecipeCreation creation)
        {
            Assert.Multiple(() =>
            {
                Assert.That(response.RecipeID,  Is.EqualTo(creation.RecipeID));
                Assert.That(response.RecipeInputs, Is.EqualTo(creation.RecipeInputs));
                Assert.That(response.RecipeOutputs, Is.EqualTo(creation.RecipeOutputs));
            });
        }
        
        private void AssertErrorListenerCalled(bool called)
        {
            Assert.That(_errorListener.WasCalled, Is.EqualTo(called));
        }

        private void AssertErrorLength(int length)
        {
            Assert.That(_errorListener.Error.RecipeCreations, Has.Length.EqualTo(length));
        }

        private void AssertError(Type exception, params RecipeCreation[] recipeCreations)
        {
            RecipeCreationError error = _errorListener.Error;
            Assert.Multiple(() =>
            {
                Assert.That(error.BaseError.Exception, Is.TypeOf<ControllerThrownException>());
                Assert.That(error.BaseError.Exception.InnerException, Is.TypeOf(exception));
                Assert.That(error.RecipeCreations, Is.EqualTo(recipeCreations));
            });
        }

        [Test]
        public void Positive_DispatchSingleCreation_CreatesRecipe_DispatchesResponse()
        {
            Assert.DoesNotThrow(() => DispatchRecipeCreations(_ringCreation));

            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_responseListener.Responses[0], _ringCreation);
        }

        [Test]
        public void Positive_DispatchMultipleCreations_CreatesRecipe_DispatchesResponses()
        {
            Assert.DoesNotThrow(() => DispatchRecipeCreations(_ringCreation, _diamondRingCreation));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(2);
            AssertResponse(_responseListener.Responses[0], _ringCreation);
            AssertResponse(_responseListener.Responses[1], _diamondRingCreation);
        }

        [Test]
        public void Negative_DispatchMultipleCreations_DuplicateRecipeID_DispatchesError()
        {
            Assert.DoesNotThrow(() => DispatchRecipeCreations(_ringCreation, _ringCreation));
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(2);
            AssertError(typeof(DuplicateEntityException), _ringCreation, _ringCreation);
        }

        [Test]
        public void Negative_DispatchSingleCreation_EmptyInputs_DispatchesError()
        {
            RecipeCreation emptyInputCreation = _ringCreation with { RecipeInputs = [] };
            
            Assert.DoesNotThrow(() => DispatchRecipeCreations(emptyInputCreation));
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError(typeof(EmptyCollectionException), emptyInputCreation);
        }
        
        [Test]
        public void Negative_DispatchSingleCreation_EmptyOutputs_DispatchesError()
        {
            RecipeCreation emptyOutputCreation = _ringCreation with { RecipeOutputs = [] };
            
            Assert.DoesNotThrow(() => DispatchRecipeCreations(emptyOutputCreation));
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertError(typeof(EmptyCollectionException), emptyOutputCreation);
        }
    }
}