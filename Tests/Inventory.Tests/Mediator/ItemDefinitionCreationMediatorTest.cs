using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Contracts.Command;
using IdelPog.Inventory.Contracts.Response;
using IdelPog.Inventory.Exceptions;
using IdelPog.Inventory.Mediator;
using Moq;

namespace IdelPog.Inventory.Tests.Mediator
{
    [TestFixture]
    public sealed class ItemDefinitionCreationMediatorTest
    {
        private ItemDefinitionCreationMediator _creationMediator;
        private Mock<IAssetRepository<ItemID, ItemDefinition>> _repositoryMock;
        private Mock<IDispatchMany<ItemDefinitionCreationResponse>> _responseDispatcherMock;

        private ItemDefinitionCreation _honeyCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _honeyCreation = new  ItemDefinitionCreation { ItemID = ItemID.HONEY, BaseSellPrice = 5, Information = new Information {Name = "", Description = "" }};
            
            _repositoryMock = new Mock<IAssetRepository<ItemID, ItemDefinition>>();
            _responseDispatcherMock = new Mock<IDispatchMany<ItemDefinitionCreationResponse>>();
            
            _creationMediator = new ItemDefinitionCreationMediator(_repositoryMock.Object, _responseDispatcherMock.Object, new CollectionAssertion(), new UniqueAssertion(), new AmountAssertion());
        }

        [SetUp]
        public void Setup()
        {
            _repositoryMock.Reset();
            _responseDispatcherMock.Reset();
        }

        private void VerifyRepositoryContains(Times times, ItemID itemID)
        {
            _repositoryMock.Verify(library => library.Contains(itemID), times);
        }
        
        private void VerifyRepositoryAdd(Times times, ItemID itemID)
        {
            _repositoryMock.Verify(library => library.Add(itemID, It.Is<ItemDefinition>(definition => definition.ItemID == itemID)), times);
        }

        private void VerifyRepositoryNoOtherCalls()
        {
            _repositoryMock.VerifyNoOtherCalls();
        }

        private void VerifyDispatcherCalled(int length)
        {
            _responseDispatcherMock.Verify(library => library.Dispatch(It.Is<ItemDefinitionCreationResponse[]>(responses => responses.Length == length)), Times.Once);
        }

        private void VerifyDispatcherNotCalled()
        {
            _responseDispatcherMock.Verify(library => library.Dispatch(It.IsAny<ItemDefinitionCreationResponse[]>()), Times.Never);
        }

        [Test]
        public void Positive_HandleMessages_CreatesDefinition_DispatchesResponse()
        {
            Assert.DoesNotThrow(() => _creationMediator.HandleMessages([_honeyCreation]));
            
            VerifyRepositoryContains(Times.Once(), _honeyCreation.ItemID);
            VerifyRepositoryAdd(Times.Once(), _honeyCreation.ItemID);
            VerifyRepositoryNoOtherCalls();
            VerifyDispatcherCalled(1);
        }

        [Test]
        public void Positive_HandleMessages_CreatesMultipleDefinitions_DispatchesResponse()
        {
            Assert.DoesNotThrow(() => _creationMediator.HandleMessages([_honeyCreation, _honeyCreation]));
            
            VerifyRepositoryContains(Times.Exactly(2), _honeyCreation.ItemID);
            VerifyRepositoryAdd(Times.Exactly(2), _honeyCreation.ItemID);
            VerifyRepositoryNoOtherCalls();
            VerifyDispatcherCalled(2);
        }

        [Test]
        public void Negative_HandleMessages_EmptyCollection_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _creationMediator.HandleMessages([]));
            
            VerifyRepositoryNoOtherCalls();
            VerifyDispatcherNotCalled();
        }
        
        [Test]
        public void Negative_HandleMessages_NullCollection_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _creationMediator.HandleMessages(null!));
            
            VerifyRepositoryNoOtherCalls();
            VerifyDispatcherNotCalled();
        }

        [Test]
        public void Negative_HandleMessages_ZeroSellPrice_Throws()
        {
            Assert.Throws<AmountZeroException>(() => _creationMediator.HandleMessages([_honeyCreation with { BaseSellPrice = 0 }]));
            
            VerifyRepositoryNoOtherCalls();
            VerifyDispatcherNotCalled();
        }

        [Test]
        public void Negative_HandleMessages_DuplicateItemID_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(_honeyCreation.ItemID)).Returns(true);
            
            Assert.Throws<DuplicateEntityException>(() => _creationMediator.HandleMessages([_honeyCreation]));
            
            _repositoryMock.Verify(library => library.Contains(_honeyCreation.ItemID), Times.Once);
            VerifyRepositoryNoOtherCalls();
            VerifyDispatcherNotCalled();
        }
    }
}