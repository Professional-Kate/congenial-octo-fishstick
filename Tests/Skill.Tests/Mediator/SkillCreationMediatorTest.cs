using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Progression;
using IdelPog.Core.Progression.Assertion;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Skill.Contracts.Command;
using IdelPog.Skill.Contracts.Response;
using IdelPog.Skill.Factory.Interface;
using IdelPog.Skill.Mediator;
using Moq;

namespace IdelPog.Skills.Tests.Mediator
{
    [TestFixture]
    public sealed class SkillCreationMediatorTest
    {
        private IBatchMediator<SkillCreation> _skillCreationMediator;
        private Mock<IStateRepository<SkillID, Skill.Contracts.Skill>> _repositoryMock;
        private Mock<ISkillCreationResponseFactory> _responseFactoryMock;
        private Mock<IDispatchMany<SkillCreationResponse>> _dispatcherMock;
        
        private SkillCreation[] _skillCreations;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryMock = new Mock<IStateRepository<SkillID, Skill.Contracts.Skill>>();
            _responseFactoryMock = new Mock<ISkillCreationResponseFactory>();
            _dispatcherMock = new Mock<IDispatchMany<SkillCreationResponse>>();

            _skillCreationMediator = new SkillCreationMediator(_repositoryMock.Object, _responseFactoryMock.Object,  _dispatcherMock.Object, new CollectionAssertion(), new UniqueAssertion(), new LevelAssertion());

            _skillCreations =
            [
                new SkillCreation
                {
                    Information = new Information { Name = "", Description = "" },
                    ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, ExperiencePerAction = 0, Level = 0, NextLevelExperience = 0 },
                    SkillID = SkillID.FORAGING
                },
                new SkillCreation
                {
                    Information = new Information { Name = "", Description = "" },
                    ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, ExperiencePerAction = 0, Level = 0, NextLevelExperience = 0 },
                    SkillID = SkillID.MINING
                }
            ];
        }

        [SetUp]
        public void Setup()
        {
            _repositoryMock.Reset();
            _responseFactoryMock.Reset();
            _dispatcherMock.Reset();
        }

        private void AssertMocksNotCalled()
        {
            _repositoryMock.VerifyNoOtherCalls();
            _responseFactoryMock.Verify(library => library.Create(_skillCreations), Times.Never);
            _dispatcherMock.Verify(library => library.Dispatch(It.IsAny<SkillCreationResponse[]>()), Times.Never);
        }

        [Test]
        public void Positive_HandleMessages_CreatesSkills()
        {
            Assert.DoesNotThrow(() => _skillCreationMediator.HandleMessages(_skillCreations));
            
            _repositoryMock.Verify(library => library.Add(It.IsAny<SkillID>(), It.IsAny<Skill.Contracts.Skill>()), Times.Exactly(_skillCreations.Length));
            _repositoryMock.Verify(library => library.Contains(It.IsAny<SkillID>()), Times.Exactly(_skillCreations.Length));
            _repositoryMock.VerifyNoOtherCalls();
            
            _responseFactoryMock.Verify(library => library.Create(_skillCreations), Times.Once);
            _dispatcherMock.Verify(library => library.Dispatch(It.IsAny<SkillCreationResponse[]>()), Times.Once);
        }

        [Test]
        public void Negative_HandleMessages_SkillAlreadyExists_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(It.IsAny<SkillID>())).Returns(true);
            
            Assert.Throws<DuplicateEntityException>(() => _skillCreationMediator.HandleMessages(_skillCreations));
            
            _repositoryMock.Verify(library => library.Contains(It.IsAny<SkillID>()), Times.Once);
            AssertMocksNotCalled();
        }

        [Test]
        public void Negative_HandleMessages_NullCollection_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _skillCreationMediator.HandleMessages(null!));
            AssertMocksNotCalled();
        }

        [Test]
        public void Negative_HandleMessages_EmptyCollection_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _skillCreationMediator.HandleMessages([]));
            AssertMocksNotCalled();
        }

        [Test]
        public void Negative_HandleMessages_BadLevelable_Throws()
        {
            SkillCreation[] skillCreations =
            [
                new()
                {
                    Information = new Information { Name = "", Description = "" },
                    ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, ExperiencePerAction = 0, Level = 120, NextLevelExperience = 0 },
                    SkillID = SkillID.FORAGING
                }
            ];
            
            Assert.Throws<MaxLevelException>(() => _skillCreationMediator.HandleMessages(skillCreations));
            
            _repositoryMock.Verify(library => library.Contains(It.IsAny<SkillID>()), Times.Once);
            AssertMocksNotCalled();
        }
    }
}