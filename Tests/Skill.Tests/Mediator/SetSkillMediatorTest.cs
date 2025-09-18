using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Information.Contracts;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Messaging.Listener.Single;
using IdelPog.Core.Progression;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.Skill.Factory.Interface;
using IdelPog.Skill.Mediator;
using IdelPog.Skill.Service.Interface;
using Moq;

namespace IdelPog.Skills.Tests.Mediator
{
    [TestFixture]
    public class SetSkillMediatorTest
    {
        private ISingleMediator<SetSkill> _setSkillMediator;
        private Mock<IStateRepository<SkillID, Skill.Contracts.Skill>> _repositoryMock;
        private Mock<ICurrentSkillSetter> _currentSkillSetterMock;
        private Mock<ISetSkillResponseFactory> _setSkillFactoryMock;
        private Mock<IDispatchOne<SetSkillResponse>> _setSkillDispatcherMock;

        private Skill.Contracts.Skill _miningSkill;
        private SetSkill _setSkill;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _miningSkill = new Skill.Contracts.Skill()
            {
                SkillID = SkillID.MINING, 
                Levelable = new Levelable(0, 0, 0, 0),
                Information = new Information { Description = "", Name = "" }
            };
            
            _setSkill = new SetSkill { SkillID = SkillID.MINING };

            _currentSkillSetterMock = new Mock<ICurrentSkillSetter>();
            _setSkillFactoryMock = new Mock<ISetSkillResponseFactory>();
            _setSkillDispatcherMock = new Mock<IDispatchOne<SetSkillResponse>>();
            _repositoryMock = new Mock<IStateRepository<SkillID, Skill.Contracts.Skill>>();

            _setSkillMediator = new SetSkillMediator(_currentSkillSetterMock.Object, _repositoryMock.Object, _setSkillFactoryMock.Object, _setSkillDispatcherMock.Object, new FoundAssertion(new ThrowHandler()));
        }

        [SetUp]
        public void SetUp()
        {
            _currentSkillSetterMock.Reset();
            _setSkillFactoryMock.Reset();
            _setSkillDispatcherMock.Reset();
            _repositoryMock.Reset();
        }

        [Test]
        public void Positive_ChangeSkill_InvokesDependencies()
        {
            _repositoryMock.Setup(library => library.Contains(_setSkill.SkillID)).Returns(true);
            _repositoryMock.Setup(library => library.Get(_setSkill.SkillID)).Returns(_miningSkill);
            
            SetSkillResponse response = new() { SkillID = _setSkill.SkillID, LevelProgress = new LevelProgress { Experience = 0, ExperiencePerAction = 0, Level = 0, NextLevelExperience = 0 }};
            _setSkillFactoryMock.Setup(library => library.Create(_miningSkill)).Returns(response);

            Assert.DoesNotThrow(() => _setSkillMediator.HandleMessage(_setSkill));

            _currentSkillSetterMock.Verify(library => library.SetCurrentSkill(_setSkill.SkillID), Times.Once);
            _setSkillFactoryMock.Verify(library => library.Create(_miningSkill), Times.Once);
            _setSkillDispatcherMock.Verify(library => library.Dispatch(response), Times.Once);
            _repositoryMock.Verify(library => library.Contains(_setSkill.SkillID), Times.Once);
            _repositoryMock.Verify(library => library.Get(_setSkill.SkillID), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_ChangeSkill_SkillNotFound_Throws()
        {
            Assert.Throws<NotFoundException<SkillID>>(() => _setSkillMediator.HandleMessage(_setSkill));
            
            _repositoryMock.Verify(library => library.Contains(_setSkill.SkillID), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();
        }
    }
}