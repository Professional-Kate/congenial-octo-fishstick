using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.HarvestNode.Contracts;
using IdelPog.HarvestNode.Contracts.Command;
using IdelPog.HarvestNode.Contracts.Response;
using IdelPog.HarvestNode.Runtime.Mediator;
using IdelPog.HarvestNode.Runtime.System.Interface;
using Moq;

namespace IdelPog.HarvestNode.Tests.Mediator
{
    [TestFixture]
    public sealed class NodeLootCreationMediatorTest
    {
        private NodeLootCreationMediator _mediator;
        private Mock<IDispatchMany<ResourceLootCreationResponse>> _responseDispatcherMock;
        private Mock<ILootTableService<ResourceID>> _lootTableServiceMock;
        private Mock<IGrantPolicyService<ResourceID>> _grantPolicyServiceMock;

        private ResourceLootCreation _singleSandCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _lootTableServiceMock = new Mock<ILootTableService<ResourceID>>();
            _grantPolicyServiceMock = new Mock<IGrantPolicyService<ResourceID>>();
            _responseDispatcherMock = new Mock<IDispatchMany<ResourceLootCreationResponse>>();
            
            _mediator = new NodeLootCreationMediator(_lootTableServiceMock.Object, _grantPolicyServiceMock.Object, _responseDispatcherMock.Object, new CollectionAssertion());

            _singleSandCreation = new ResourceLootCreation
            {
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
            _responseDispatcherMock.Reset();
            _lootTableServiceMock.Reset();
            _grantPolicyServiceMock.Reset();
        }

        private void VerifyLootServiceCalled(Times times)
        {
            _lootTableServiceMock.Verify(library => library.CreateLootTable(It.IsAny<LootTableEntry[]>(), It.IsAny<ResourceID>()), times);
        }
        
        private void VerifyGrantServiceCalled(Times times)
        {
            _grantPolicyServiceMock.Verify(library => library.CreateGrantPolicy(It.IsAny<GrantPolicyEntry>(),It.IsAny<ResourceID>()), times);
        }

        private void VerifyDispatcherCalled()
        {
            _responseDispatcherMock.Verify(library => library.Dispatch(It.IsAny<ResourceLootCreationResponse[]>()), Times.Once);
        }

        private void VerifyDispatcherNotCalled()
        {
            _responseDispatcherMock.VerifyNoOtherCalls();
        }
        
        [Test]
        public void Positive_HandleMessages_MultipleMessages_CreatesLootTable_ReturnsResponse()
        {
            Assert.DoesNotThrow(() => _mediator.HandleMessages([_singleSandCreation, _singleSandCreation]));

            VerifyLootServiceCalled(Times.Exactly(2));
            VerifyGrantServiceCalled(Times.Exactly(2));
            VerifyDispatcherCalled();
        }

        [Test]
        public void Positive_HandleMessages_SingleMessage_ZeroWeightGrantPolicy_ReturnsResponse()
        {
            ResourceLootCreation creation = _singleSandCreation with
            {
                GrantPolicyEntry = new GrantPolicyEntry { GrantWeight = 0, SkipWeight = 0 }
            };
            
            Assert.DoesNotThrow(() => _mediator.HandleMessages([creation]));

            VerifyLootServiceCalled(Times.Once());
            VerifyGrantServiceCalled(Times.Once());
            VerifyDispatcherCalled();
        }

        [Test]
        public void Positive_HandleMessages_SingleMessage_MultipleLootTableEntries_ReturnsResponse()
        {
            ResourceLootCreation creation = _singleSandCreation with
            {
                LootTableEntries = [ new LootTableEntry { ItemID = ItemID.SAND, Weight = 5 }, new LootTableEntry { ItemID = ItemID.WATER, Weight = 1 } ]
            };
            
            Assert.DoesNotThrow(() => _mediator.HandleMessages([creation]));

            VerifyLootServiceCalled(Times.Once());
            VerifyGrantServiceCalled(Times.Once());
            VerifyDispatcherCalled();
        }

        [Test]
        public void Negative_HandleMessages_EmptyLootTableEntries_Throws()
        {
            EmptyCollectionException exception = Assert.Throws<EmptyCollectionException>(() => _mediator.HandleMessages([_singleSandCreation with { LootTableEntries = [] }]));
            Assert.That(exception.CollectionType, Is.EqualTo(typeof(LootTableEntry)));
            
            VerifyLootServiceCalled(Times.Never());
            VerifyGrantServiceCalled(Times.Never());
            VerifyDispatcherNotCalled();
        }

        [Test]
        public void Negative_HandleMessages_LootTableAlreadyExists_Throws()
        {
            _lootTableServiceMock.Setup(library => library.CreateLootTable(_singleSandCreation.LootTableEntries, _singleSandCreation.ResourceID))
                .Throws(new DuplicateEntityException(_singleSandCreation.ResourceID));
            
            Assert.Throws<DuplicateEntityException>(() => _mediator.HandleMessages([_singleSandCreation]));

            VerifyLootServiceCalled(Times.Once());
            VerifyGrantServiceCalled(Times.Never());
            VerifyDispatcherNotCalled();
        }

        [Test]
        public void Negative_HandleMessages_CollectionNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _mediator.HandleMessages(null!));
            
            VerifyLootServiceCalled(Times.Never());
            VerifyGrantServiceCalled(Times.Never());
            VerifyDispatcherNotCalled();
        }
        
        [Test]
        public void Negative_HandleMessages_CollectionEmpty_Throws()
        {
            EmptyCollectionException exception = Assert.Throws<EmptyCollectionException>(() => _mediator.HandleMessages([]));
            Assert.That(exception.CollectionType, Is.EqualTo(typeof(ResourceLootCreation)));
            
            VerifyLootServiceCalled(Times.Never());
            VerifyGrantServiceCalled(Times.Never());
            VerifyDispatcherNotCalled();
        }
    }
}