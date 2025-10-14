using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Contracts.Command;
using IdelPog.Inventory.Contracts.Response;
using IdelPog.Inventory.Crafting.ECS;
using IdelPog.Inventory.Crafting.Factory.Interface;
using IdelPog.Inventory.Crafting.Mediator;
using Moq;

namespace IdelPog.Inventory.Tests.Crafting.Mediator
{
    [TestFixture]
    public sealed class RecipeCreationMediatorTest
    {
        private IBatchMediator<RecipeCreation> _creationMediator;
        private Mock<IAssetRepository<RecipeID, CraftingRecipeEntity>> _repositoryEntityMock;
        private Mock<ICraftingRecipeEntityFactory> _factoryMock;
        private Mock<IDispatchMany<RecipeCreationResponse>> _responseDispatchMock;

        private RecipeCreation _ironRingCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _ironRingCreation = new RecipeCreation
            {
                RecipeID = RecipeID.IRON_RING,
                RecipeInputs = [new RecipeInput { ItemID = ItemID.IRON, Amount = 3}],
                RecipeOutputs = [new RecipeOutput { ItemID = ItemID.RING, Amount = 1 }]
            };
            
            _repositoryEntityMock = new Mock<IAssetRepository<RecipeID, CraftingRecipeEntity>>();
            _factoryMock = new Mock<ICraftingRecipeEntityFactory>();
            _responseDispatchMock = new Mock<IDispatchMany<RecipeCreationResponse>>();

            _creationMediator = new RecipeCreationMediator(_repositoryEntityMock.Object, _factoryMock.Object, _responseDispatchMock.Object, new CollectionAssertion(), new UniqueAssertion());
        }

        [SetUp]
        public void Setup()
        {
            _repositoryEntityMock.Reset();
            _factoryMock.Reset();
            _responseDispatchMock.Reset();
        }

        private void VerifyRepositoryCalled(Times times)
        {
            _repositoryEntityMock.Verify(library => library.Contains(It.IsAny<RecipeID>()), times);
            _repositoryEntityMock.Verify(library => library.Add(It.IsAny<RecipeID>(), It.IsAny<CraftingRecipeEntity>()), times);
            VerifyRepositoryNoOtherCalls();
        }

        private void VerifyRepositoryNoOtherCalls()
        {
            _repositoryEntityMock.VerifyNoOtherCalls();
        }
        
        private void VerifyDispatcherCalled(Times times)
        {
            _responseDispatchMock.Verify(library => library.Dispatch(It.IsAny<RecipeCreationResponse[]>()), times);
        }

        private void VerifyFactoryCalled(Times times)
        {
            _factoryMock.Verify(library => library.Create(It.IsAny<RecipeInput[]>(), It.IsAny<RecipeOutput[]>()), times);
        }

        [Test]
        public void Positive_HandleMessages_CreatesNewEntity()
        {
            Assert.DoesNotThrow(() => _creationMediator.HandleMessages([_ironRingCreation]));
            
            VerifyDispatcherCalled(Times.Once());
            VerifyFactoryCalled(Times.Once());
            VerifyRepositoryCalled(Times.Once());
        }

        [Test]
        public void Positive_HandleMessages_MultipleMessages_SingleDispatch()
        {
            Assert.DoesNotThrow(() => _creationMediator.HandleMessages([_ironRingCreation, _ironRingCreation]));
            
            VerifyDispatcherCalled(Times.Once());
            VerifyFactoryCalled(Times.Exactly(2));
            VerifyRepositoryCalled(Times.Exactly(2));
        }

        [Test]
        public void Negative_HandleMessages_EmptyCollection_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _creationMediator.HandleMessages([]));
            
            VerifyDispatcherCalled(Times.Never());
            VerifyFactoryCalled(Times.Never());
            VerifyRepositoryCalled(Times.Never());
        }
        
        [Test]
        public void Negative_HandleMessages_NullCollection_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _creationMediator.HandleMessages(null!));
            
            VerifyDispatcherCalled(Times.Never());
            VerifyFactoryCalled(Times.Never());
            VerifyRepositoryCalled(Times.Never());
        }

        [Test]
        public void Negative_HandleMessages_EmptyInputs_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _creationMediator.HandleMessages([_ironRingCreation with { RecipeInputs = []}]));
            
            VerifyDispatcherCalled(Times.Never());
            VerifyFactoryCalled(Times.Never());
            VerifyRepositoryCalled(Times.Never());
        }
        
        [Test]
        public void Negative_HandleMessages_NullInputs_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _creationMediator.HandleMessages([_ironRingCreation with { RecipeInputs = null!}]));
            
            VerifyDispatcherCalled(Times.Never());
            VerifyFactoryCalled(Times.Never());
            VerifyRepositoryCalled(Times.Never());
        }
        
        [Test]
        public void Negative_HandleMessages_EmptyOutput_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _creationMediator.HandleMessages([_ironRingCreation with { RecipeOutputs = []}]));
            
            VerifyDispatcherCalled(Times.Never());
            VerifyFactoryCalled(Times.Never());
            VerifyRepositoryCalled(Times.Never());
        }
        
        [Test]
        public void Negative_HandleMessages_NullOutput_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _creationMediator.HandleMessages([_ironRingCreation with { RecipeOutputs = null!}]));
            
            VerifyDispatcherCalled(Times.Never());
            VerifyFactoryCalled(Times.Never());
            VerifyRepositoryCalled(Times.Never());
        }

        [Test]
        public void Negative_HandleMessages_NonUniqueRecipeID_Throws()
        {
            _repositoryEntityMock.Setup(library => library.Contains(_ironRingCreation.RecipeID)).Returns(true);
            
            Assert.Throws<DuplicateEntityException>(() => _creationMediator.HandleMessages([_ironRingCreation]));
            
            VerifyDispatcherCalled(Times.Never());
            VerifyFactoryCalled(Times.Never());
            
            _repositoryEntityMock.Verify(library => library.Contains(_ironRingCreation.RecipeID), Times.Once);
            VerifyRepositoryNoOtherCalls();
        }
    }
}