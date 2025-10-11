using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.HarvestNode.Runtime.Factory.Interface;
using IdelPog.HarvestNode.Runtime.Mediator;
using IdelPog.Progression.Runtime;
using Moq;

namespace IdelPog.HarvestNode.Tests.Mediator
{
    [TestFixture]
    public class NodeRequirementsCreationMediatorTest
    {
        private IBatchMediator<HarvestNodeRequirementsCreation> _creationMediator;
        private Mock<IAssetRepository<SkillID, UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse>>> _repositoryMock;
        private Mock<IUnlockRequirementsEntityFactory> _factoryMock;
        private Mock<IDispatchMany<HarvestNodeRequirementsCreationResponse>> _responseDispatcherMock;

        private HarvestNodeRequirementsCreation _miningCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryMock = new Mock<IAssetRepository<SkillID, UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse>>>();
            _factoryMock = new Mock<IUnlockRequirementsEntityFactory>();
            _responseDispatcherMock = new Mock<IDispatchMany<HarvestNodeRequirementsCreationResponse>>();
            ThrowHandler throwHandler = new();

            _creationMediator = new NodeRequirementsCreationMediator(_repositoryMock.Object, _factoryMock.Object, _responseDispatcherMock.Object, new CollectionAssertion(throwHandler), new UniqueAssertion(throwHandler));

            _miningCreation = new HarvestNodeRequirementsCreation
            {
                SkillID = SkillID.MINING,
                HarvestNodeRequirements =
                [
                    new HarvestNodeRequirement
                    {
                        ItemID = ItemID.STONE, 
                        RequiredLevel = 1,
                        OnUnlockCommand = new HarvestNodeUnlockResponse { ResourceID = ResourceID.STONE, SkillID = SkillID.MINING }
                    },
                    new HarvestNodeRequirement
                    {
                        ItemID = ItemID.IRON, 
                        RequiredLevel = 2,
                        OnUnlockCommand = new HarvestNodeUnlockResponse { ResourceID = ResourceID.IRON_CLUSTER, SkillID = SkillID.MINING }
                    },
                    new HarvestNodeRequirement
                    {
                        ItemID = ItemID.COPPER, 
                        RequiredLevel = 3,
                        OnUnlockCommand = new HarvestNodeUnlockResponse { ResourceID = ResourceID.COPPER_CLUSTER, SkillID = SkillID.MINING }
                    },
                    new HarvestNodeRequirement
                    {
                        ItemID = ItemID.GOLD, 
                        RequiredLevel = 4,
                        OnUnlockCommand = new HarvestNodeUnlockResponse { ResourceID = ResourceID.GOLD_CLUSTER, SkillID = SkillID.MINING }
                    }
                ]
            };
        }

        [SetUp]
        public void Setup()
        {
            _repositoryMock.Reset();
            _factoryMock.Reset();
            _responseDispatcherMock.Reset();
        }

        private void VerifyNoMoreMockCalls()
        { 
            _factoryMock.VerifyNoOtherCalls();
            _responseDispatcherMock.VerifyNoOtherCalls();
            _repositoryMock.VerifyNoOtherCalls();
        }
        
        private void VerifyRepository(SkillID skillID)
        {
            _repositoryMock.Verify(library => library.Contains(skillID), Times.Once);
            _repositoryMock.Verify(library => library.Add(skillID, It.IsAny<UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse>>()), Times.Once);
        }
        
        private void VerifyFactory(HarvestNodeRequirementsCreation creation)
        {
            _factoryMock.Verify(library => library.Create(creation.SkillID, creation.HarvestNodeRequirements), Times.Once);
        }

        private void VerifyDispatcher()
        {
            _responseDispatcherMock.Verify(library => library.Dispatch(It.IsAny<HarvestNodeRequirementsCreationResponse[]>()), Times.Once);
        }

        [Test]
        public void Positive_HandleMessages_CreationSuccessful_DispatchesResponse()
        {
            Assert.DoesNotThrow(() => _creationMediator.HandleMessages([_miningCreation]));
            
            VerifyRepository(_miningCreation.SkillID);
            VerifyFactory(_miningCreation);
            VerifyDispatcher();
            
            VerifyNoMoreMockCalls();
        }

        [Test]
        public void Positive_HandleMessages_MultipleMessages_DispatchesResponses()
        {
            HarvestNodeRequirementsCreation foragingCreation = _miningCreation with { SkillID = SkillID.FORAGING };
            Assert.DoesNotThrow(() => _creationMediator.HandleMessages([_miningCreation, foragingCreation]));
            
            VerifyRepository(_miningCreation.SkillID);
            VerifyRepository(foragingCreation.SkillID);
            
            VerifyFactory(_miningCreation);
            VerifyFactory(foragingCreation);
            VerifyDispatcher();
            
            VerifyNoMoreMockCalls();
        }

        [Test]
        public void Negative_HandleMessages_EmptyArray_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _creationMediator.HandleMessages([]));

            VerifyNoMoreMockCalls();
        }
        
        [Test]
        public void Negative_HandleMessages_NullArray_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _creationMediator.HandleMessages(null!));
            
            VerifyNoMoreMockCalls();
        }

        [Test]
        public void Negative_HandleMessages_EmptyHarvestNodeRequirements_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _creationMediator.HandleMessages([_miningCreation with { HarvestNodeRequirements = []}]));
            
            VerifyNoMoreMockCalls();
        }

        [Test]
        public void Negative_HandleMessages_DuplicateSkillID_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(_miningCreation.SkillID)).Returns(true);
            
            Assert.Throws<DuplicateEntityException>(() => _creationMediator.HandleMessages([_miningCreation]));
            
            _repositoryMock.Verify(library => library.Contains(_miningCreation.SkillID), Times.Once);
            VerifyNoMoreMockCalls();
        }
    }
}