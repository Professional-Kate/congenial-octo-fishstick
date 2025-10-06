using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Information.Contracts;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Progression;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.HarvestNode.Factory.Interface;
using IdelPog.HarvestNode.Runtime.ECS;
using IdelPog.HarvestNode.Runtime.Factory.Interfaces;
using IdelPog.HarvestNode.Runtime.Mediator;
using Moq;

namespace IdelPog.HarvestNode.Tests.Mediator
{
    [TestFixture]
    public sealed class NodeCreationMediatorTest
    {
        private IBatchMediator<HarvestNodeCreation> _nodeCreationMediator;
        private Mock<IAssetRepository<SkillID, SkillNodeEntity>> _skillNodeEntityRepositoryMock;
        private Mock<IStateRepository<ItemID, Contracts.HarvestNode>> _harvestNodeRepositoryMock;
        private Mock<ISkillNodeEntityFactory> _skillNodeEntityFactoryMock;
        private Mock<IHarvestNodeFactory> _harvestNodeFactoryMock;
        private Mock<INodeCreationResponseFactory> _nodeCreationResponseFactoryMock;
        private Mock<IDispatchMany<HarvestNodeCreationResponse>> _dispatchOneMock;

        private HarvestNodeCreation _miningCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _skillNodeEntityRepositoryMock = new Mock<IAssetRepository<SkillID, SkillNodeEntity>>();
            _harvestNodeRepositoryMock = new Mock<IStateRepository<ItemID, Contracts.HarvestNode>>();
            _skillNodeEntityFactoryMock = new Mock<ISkillNodeEntityFactory>();
            _harvestNodeFactoryMock = new Mock<IHarvestNodeFactory>();
            _nodeCreationResponseFactoryMock = new Mock<INodeCreationResponseFactory>();
            _dispatchOneMock = new Mock<IDispatchMany<HarvestNodeCreationResponse>>();

            _miningCreation = new HarvestNodeCreation
            {
                ReadOnlyHarvestNodes =
                [
                    new ReadOnlyHarvestNode { ItemID =  ItemID.STONE, ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 }, Information = new Information { Name = "", Description = "" }, HarvestNodeID = HarvestNodeID.ROCK},
                    new ReadOnlyHarvestNode { ItemID =  ItemID.COPPER, ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 }, Information = new Information { Name = "", Description = "" }, HarvestNodeID = HarvestNodeID.COPPER_CLUSTER},
                    new ReadOnlyHarvestNode { ItemID =  ItemID.GOLD, ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 }, Information = new Information { Name = "", Description = "" }, HarvestNodeID = HarvestNodeID.GOLD_CLUSTER},
                    new ReadOnlyHarvestNode { ItemID =  ItemID.IRON, ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, Level = 0, ExperiencePerAction = 0, NextLevelExperience = 0 }, Information = new Information { Name = "", Description = "" }, HarvestNodeID = HarvestNodeID.IRON_CLUSTER}
                ],
                LinkedSkill = SkillID.MINING
            };

            _nodeCreationMediator = new NodeCreationMediator(_harvestNodeRepositoryMock.Object, _skillNodeEntityRepositoryMock.Object, _skillNodeEntityFactoryMock.Object, _harvestNodeFactoryMock.Object, _nodeCreationResponseFactoryMock.Object, _dispatchOneMock.Object, new UniqueAssertion(new ThrowHandler()), new CollectionAssertion(new ThrowHandler()));
        }

        [SetUp]
        public void Setup()
        {
            _skillNodeEntityRepositoryMock.Reset();
            _harvestNodeRepositoryMock.Reset();
            _skillNodeEntityFactoryMock.Reset();
            _harvestNodeFactoryMock.Reset();
            _nodeCreationResponseFactoryMock.Reset();
            _dispatchOneMock.Reset();
        }

        [Test]
        public void Positive_HandleMessages_SkillNotFound_CreatesSkillAndNodes()
        {
            Assert.DoesNotThrow(() => _nodeCreationMediator.HandleMessages([_miningCreation]));
            
            _skillNodeEntityRepositoryMock.Verify(library => library.Add(_miningCreation.LinkedSkill, It.IsAny<SkillNodeEntity>()), Times.Once);
            _harvestNodeRepositoryMock.Verify(library => library.Add(It.IsIn(ItemID.STONE, ItemID.COPPER, ItemID.GOLD, ItemID.IRON), It.IsAny<Contracts.HarvestNode>()), Times.Exactly(_miningCreation.ReadOnlyHarvestNodes.Length));
            _skillNodeEntityFactoryMock.Verify(library => library.Create(_miningCreation.LinkedSkill, _miningCreation.ReadOnlyHarvestNodes), Times.Once);
            _harvestNodeFactoryMock.Verify(library => library.Create(It.IsAny<ReadOnlyHarvestNode>()), Times.Exactly(_miningCreation.ReadOnlyHarvestNodes.Length));
            _dispatchOneMock.Verify(library => library.Dispatch(It.IsAny<HarvestNodeCreationResponse[]>()), Times.Once);
        }

        [Test]
        public void Negative_HandleMessages_SkillAlreadyExists_Throws()
        {
            _skillNodeEntityRepositoryMock.Setup(library => library.Contains(It.IsAny<SkillID>())).Returns(true);
            
            Assert.Throws<DuplicateEntityException>(() => _nodeCreationMediator.HandleMessages([_miningCreation]));
            
            _dispatchOneMock.Verify(library => library.Dispatch(It.IsAny<HarvestNodeCreationResponse[]>()), Times.Never);
            _skillNodeEntityRepositoryMock.Verify(library => library.Add(_miningCreation.LinkedSkill, It.IsAny<SkillNodeEntity>()), Times.Never);
            _harvestNodeRepositoryMock.Verify(library => library.Add(It.IsIn(ItemID.STONE, ItemID.COPPER, ItemID.GOLD, ItemID.IRON), It.IsAny<Contracts.HarvestNode>()), Times.Never);
        }

        [Test]
        public void Negative_HandleMessages_ResourceIDAlreadyExists_Throws()
        {
            _harvestNodeRepositoryMock.Setup(library => library.Contains(It.IsAny<ItemID>())).Returns(true);
            
            Assert.Throws<DuplicateEntityException>(() => _nodeCreationMediator.HandleMessages([_miningCreation]));
            
            _dispatchOneMock.Verify(library => library.Dispatch(It.IsAny<HarvestNodeCreationResponse[]>()), Times.Never);
            _skillNodeEntityRepositoryMock.Verify(library => library.Add(_miningCreation.LinkedSkill, It.IsAny<SkillNodeEntity>()), Times.Never);
            _harvestNodeRepositoryMock.Verify(library => library.Add(It.IsIn(ItemID.STONE, ItemID.COPPER, ItemID.GOLD, ItemID.IRON), It.IsAny<Contracts.HarvestNode>()), Times.Never);
            
            
        }
    }
}