using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.HarvestNode.Runtime.Factory.Interface;
using IdelPog.HarvestNode.Runtime.Mediator;
using IdelPog.Loot.Policy;
using IdelPog.Loot.Table;
using Moq;

namespace IdelPog.HarvestNode.Tests.Mediator
{
    [TestFixture]
    public sealed class NodeLootGenerationMediatorTest
    {
        private NodeLootGenerationMediator _mediator;
        private Mock<IAssetRepository<ItemID, ILootTable>> _lootTableRepositoryMock;
        private Mock<IAssetRepository<ItemID,IGrantPolicy>> _grantPolicyRepositoryMock;
        private Mock<IWeightedLootTableFactory> _lootTableFactoryMock;
        private Mock<IWeightedPolicyFactory> _policyFactoryMock;
        private Mock<IDispatchMany<HarvestNodeLootCreationResponse>> _responseDispatcherMock;

        private HarvestNodeLootCreation _singleSandCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _lootTableRepositoryMock = new Mock<IAssetRepository<ItemID, ILootTable>>();
            _grantPolicyRepositoryMock = new Mock<IAssetRepository<ItemID, IGrantPolicy>>();
            _lootTableFactoryMock = new Mock<IWeightedLootTableFactory>();
            _policyFactoryMock =  new Mock<IWeightedPolicyFactory>();
            _responseDispatcherMock = new Mock<IDispatchMany<HarvestNodeLootCreationResponse>>();
            ThrowHandler throwHandler = new();
            
            _mediator = new NodeLootGenerationMediator(_lootTableRepositoryMock.Object, _grantPolicyRepositoryMock.Object, _lootTableFactoryMock.Object, _policyFactoryMock.Object, _responseDispatcherMock.Object, new CollectionAssertion(throwHandler), new UniqueAssertion(throwHandler));

            _singleSandCreation = new HarvestNodeLootCreation
            {
                ItemID = ItemID.SAND,
                ResourceID = ResourceID.RIVER,
                LootTableEntries =
                [
                    new LootTableEntry { ItemID = ItemID.SAND, Weight = 1 }
                ],
                GrantPolicyEntry = new GrantPolicyEntry { GrantWeight = 1, SkipWeight = 0 }
            };
        }

        [SetUp]
        public void Setup()
        {
            _lootTableRepositoryMock.Reset();
            _responseDispatcherMock.Reset();
            _grantPolicyRepositoryMock.Reset();
        }

        private void VerifyDispatcherCalled()
        {
            _responseDispatcherMock.Verify(library => library.Dispatch(It.IsAny<HarvestNodeLootCreationResponse[]>()), Times.Once);
        }

        private void VerifyDispatcherNotCalled()
        {
            _responseDispatcherMock.VerifyNoOtherCalls();
        }
        
        private void AssertTableRepositoryContains(int amountCalled)
        {
            _lootTableRepositoryMock.Verify(library => library.Contains(It.IsAny<ItemID>()), Times.Exactly(amountCalled));
        }

        private void AssertTableRepositoryAdd(int amountCalled)
        {
            _lootTableRepositoryMock.Verify(library => library.Add(It.IsAny<ItemID>(), It.IsAny<ILootTable>()), Times.Exactly(amountCalled));
        }

        private void VerifyNoMoreTableRepositoryCalls()
        {
            _lootTableRepositoryMock.VerifyNoOtherCalls();
        }
        
        private void AssertPolicyRepositoryContains(int amountCalled)
        {
            _grantPolicyRepositoryMock.Verify(library => library.Contains(It.IsAny<ItemID>()), Times.Exactly(amountCalled));
        }

        private void AssertPolicyRepositoryAdd(int amountCalled)
        {
            _grantPolicyRepositoryMock.Verify(library => library.Add(It.IsAny<ItemID>(), It.IsAny<IGrantPolicy>()), Times.Exactly(amountCalled));
        }

        private void VerifyNoMorePolicyRepositoryCalls()
        {
            _grantPolicyRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_HandleMessages_MultipleMessages_CreatesLootTable_ReturnsResponse()
        {
            Assert.DoesNotThrow(() => _mediator.HandleMessages([_singleSandCreation, _singleSandCreation]));

            AssertTableRepositoryContains(2);
            AssertTableRepositoryAdd(2);
            VerifyNoMoreTableRepositoryCalls();
            AssertPolicyRepositoryContains(2);
            AssertPolicyRepositoryAdd(2);
            VerifyNoMorePolicyRepositoryCalls();
            VerifyDispatcherCalled();
        }

        [Test]
        public void Positive_HandleMessages_SingleMessage_ZeroWeightGrantPolicy_ReturnsResponse()
        {
            HarvestNodeLootCreation creation = _singleSandCreation with
            {
                GrantPolicyEntry = new GrantPolicyEntry { GrantWeight = 0, SkipWeight = 0 }
            };
            
            Assert.DoesNotThrow(() => _mediator.HandleMessages([creation]));

            AssertTableRepositoryContains(1);
            AssertTableRepositoryAdd(1);
            VerifyNoMoreTableRepositoryCalls();
            AssertPolicyRepositoryContains(1);
            AssertPolicyRepositoryAdd(1);
            VerifyNoMorePolicyRepositoryCalls();
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

            AssertTableRepositoryContains(1);
            AssertTableRepositoryAdd(1);
            VerifyNoMoreTableRepositoryCalls();
            AssertPolicyRepositoryContains(1);
            AssertPolicyRepositoryAdd(1);
            VerifyNoMorePolicyRepositoryCalls();
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