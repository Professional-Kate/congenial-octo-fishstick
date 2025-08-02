using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Common.Factories;
using IdelPog.Common.Responses;
using IdelPog.Messaging.Dispatch.Single;
using IdelPog.Messaging.Listeners.Single;
using IdelPog.SimulationEngine.Skill;
using Moq;

namespace IdelPogTests.Orchestration
{
    [TestFixture]
    public class SetSkillMediatorTest
    {
        private ISingleMediator<SetSkill> _setSkillMediator;
        private Mock<ICurrentSkillSetter> _currentSkillSetterMock;
        private Mock<ISetSkillResponseFactory> _setSkillFactoryMock;
        private Mock<IDispatchOne<SetSkillResponse>> _setSkillDispatcherMock;

        private SetSkill _setSkill;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _setSkill = new SetSkill { SkillID = SkillID.MINING };

            _currentSkillSetterMock = new Mock<ICurrentSkillSetter>();
            _setSkillFactoryMock = new Mock<ISetSkillResponseFactory>();
            _setSkillDispatcherMock = new Mock<IDispatchOne<SetSkillResponse>>();

            _setSkillMediator = new SetSkillMediator(_currentSkillSetterMock.Object, _setSkillFactoryMock.Object, _setSkillDispatcherMock.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _currentSkillSetterMock.Reset();
            _setSkillFactoryMock.Reset();
            _setSkillDispatcherMock.Reset();
        }

        [Test]
        public void Positive_ChangeSkill_InvokesDependencies()
        {
            SetSkillResponse response = new() { SkillID = _setSkill.SkillID };
            _setSkillFactoryMock.Setup(library => library.Create(_setSkill)).Returns(response);

            Assert.DoesNotThrow(() => _setSkillMediator.HandleMessage(_setSkill));

            _currentSkillSetterMock.Verify(library => library.SetCurrentSkill(_setSkill.SkillID), Times.Once);
            _setSkillFactoryMock.Verify(library => library.Create(_setSkill), Times.Once);
            _setSkillDispatcherMock.Verify(library => library.Dispatch(response), Times.Once);
        }

        [Test]
        public void Positive_ChangeSkill_DoesNotSuppressExceptions()
        {
            _setSkillFactoryMock.Setup(library => library.Create(_setSkill)).Throws<Exception>();

            Assert.Throws<Exception>(() => _setSkillMediator.HandleMessage(_setSkill));
        }
    }
}