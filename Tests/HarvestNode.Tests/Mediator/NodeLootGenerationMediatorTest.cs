using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.HarvestNode.Runtime.Factory.Interfaces;
using IdelPog.HarvestNode.Runtime.Mediator;
using IdelPog.Loot.Table;
using Moq;

namespace IdelPog.HarvestNode.Tests.Mediator
{
    [TestFixture]
    public sealed class NodeLootGenerationMediatorTest
    {
        private NodeLootGenerationMediator _mediator;
        private Mock<IAssetRepository<ItemID, ILootTable>> _lootTableRepositoryMock;
        private Mock<IWeightedLootTableFactory> _lootTableFactoryMock;
        private Mock<IDispatchMany<HarvestNodeLootCreationResponse>> _responseDispatcherMock;

        private HarvestNodeLootCreation _singleSandCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _lootTableRepositoryMock = new Mock<IAssetRepository<ItemID, ILootTable>>();
            _lootTableFactoryMock = new Mock<IWeightedLootTableFactory>();
            _responseDispatcherMock = new Mock<IDispatchMany<HarvestNodeLootCreationResponse>>();
            ThrowHandler throwHandler = new();
            
            _mediator = new NodeLootGenerationMediator(_lootTableRepositoryMock.Object, _lootTableFactoryMock.Object, _responseDispatcherMock.Object, new CollectionAssertion(throwHandler), new UniqueAssertion(throwHandler));

            _singleSandCreation = new HarvestNodeLootCreation
            {
                ItemID = ItemID.SAND,
                ResourceID = ResourceID.RIVER,
                LootTableEntries =
                [
                    new LootTableEntry { ItemID = ItemID.SAND, Weight = 1 }
                ]
            };
        }

        [SetUp]
        public void Setup()
        {
            _lootTableRepositoryMock.Reset();
            _responseDispatcherMock.Reset();
        }

        private void VerifyDispatcherCalled()
        {
            _responseDispatcherMock.Verify(library => library.Dispatch(It.IsAny<HarvestNodeLootCreationResponse[]>()), Times.Once);
        }

        private void VerifyDispatcherNotCalled()
        {
            _responseDispatcherMock.VerifyNoOtherCalls();
        }
        
        private void AssertRepositoryContains(int amountCalled)
        {
            _lootTableRepositoryMock.Verify(library => library.Contains(It.IsAny<ItemID>()), Times.Exactly(amountCalled));
        }

        private void AssertRepositoryAdd(int amountCalled)
        {
            _lootTableRepositoryMock.Verify(library => library.Add(It.IsAny<ItemID>(), It.IsAny<ILootTable>()), Times.Exactly(amountCalled));
        }

        private void VerifyNoMoreRepositoryCalls()
        {
            _lootTableRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_HandleMessages_MultipleMessages_CreatesLootTable_ReturnsResponse()
        {
            Assert.DoesNotThrow(() => _mediator.HandleMessages([_singleSandCreation, _singleSandCreation]));

            AssertRepositoryContains(2);
            AssertRepositoryAdd(2);
            VerifyNoMoreRepositoryCalls();
            VerifyDispatcherCalled();
        }

        [Test]
        public void Positive_HandleMessages_SingleMessage_MultipleLootTableEntries_ReturnsResponse()
        {
            HarvestNodeLootCreation creation = _singleSandCreation with
            {
                LootTableEntries = [ new LootTableEntry { ItemID = ItemID.SAND, Weight = 5 }, new LootTableEntry { ItemID = ItemID.WATER, Weight = 1 } ]
            };
            
            Assert.DoesNotThrow(() => _mediator.HandleMessages([creation]));

            AssertRepositoryContains(1);
            AssertRepositoryAdd(1);
            VerifyNoMoreRepositoryCalls();
            VerifyDispatcherCalled();
        }

        [Test]
        public void Negative_HandleMessages_EmptyLootTableEntries_Throws()
        {
            EmptyCollectionException exception = Assert.Throws<EmptyCollectionException>(() => _mediator.HandleMessages([_singleSandCreation with { LootTableEntries = [] }]));
            Assert.That(exception.CollectionType, Is.EqualTo(typeof(LootTableEntry)));
            
            VerifyDispatcherNotCalled();
        }

        [Test]
        public void Negative_HandleMessages_LootTableAlreadyExists_Throws()
        {
            _lootTableRepositoryMock.Setup(library => library.Contains(_singleSandCreation.ItemID)).Returns(true);
            
            DuplicateEntityException exception = Assert.Throws<DuplicateEntityException>(() => _mediator.HandleMessages([_singleSandCreation]));
            Assert.That(exception.ID, Is.EqualTo(_singleSandCreation.ItemID));

            VerifyDispatcherNotCalled();
        }

        [Test]
        public void Negative_HandleMessages_CollectionNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _mediator.HandleMessages(null!));
            
            VerifyDispatcherNotCalled();
            
        }
        
        [Test]
        public void Negative_HandleMessages_CollectionEmpty_Throws()
        {
            EmptyCollectionException exception = Assert.Throws<EmptyCollectionException>(() => _mediator.HandleMessages([]));
            Assert.That(exception.CollectionType, Is.EqualTo(typeof(HarvestNodeLootCreation)));
            
            VerifyDispatcherNotCalled();
        }
    }
}