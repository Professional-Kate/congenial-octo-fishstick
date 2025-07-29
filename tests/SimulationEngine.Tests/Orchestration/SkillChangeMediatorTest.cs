using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Common.Factories;
using IdelPog.Common.Responses;
using IdelPog.Messaging.Dispatch;
using IdelPog.SimulationEngine.Skill;
using Moq;

namespace IdelPogTests.Orchestration
{
    [TestFixture]
    public class SkillChangeMediatorTest
    {
        private ISkillChangeMediator _skillChangeMediator;
        private Mock<ICurrentSkillSetter> _currentSkillSetterMock;
        private Mock<ISkillChangeResponseFactory> _skillChangeFactoryMock;
        private Mock<IDispatchOne<SkillChangeResponse>> _skillChangeDispatcherMock;

        private SkillChange _skillChange;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _skillChange = new SkillChange { SkillID = SkillID.MINING };

            _currentSkillSetterMock = new Mock<ICurrentSkillSetter>();
            _skillChangeFactoryMock = new Mock<ISkillChangeResponseFactory>();
            _skillChangeDispatcherMock = new Mock<IDispatchOne<SkillChangeResponse>>();

            _skillChangeMediator = new SkillChangeMediator(_currentSkillSetterMock.Object, _skillChangeFactoryMock.Object, _skillChangeDispatcherMock.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _currentSkillSetterMock.Reset();
            _skillChangeFactoryMock.Reset();
            _skillChangeDispatcherMock.Reset();
        }

        [Test]
        public void Positive_ChangeSkill_InvokesDependencies()
        {
            SkillChangeResponse response = new() { SkillID = _skillChange.SkillID };
            _skillChangeFactoryMock.Setup(library => library.Create(_skillChange)).Returns(response);

            Assert.DoesNotThrow(() => _skillChangeMediator.ChangeSkill(_skillChange));

            _currentSkillSetterMock.Verify(library => library.SetCurrentSkill(_skillChange.SkillID), Times.Once);
            _skillChangeFactoryMock.Verify(library => library.Create(_skillChange), Times.Once);
            _skillChangeDispatcherMock.Verify(library => library.Dispatch(response), Times.Once);
        }

        [Test]
        public void Positive_ChangeSkill_DoesNotSuppressExceptions()
        {
            _skillChangeFactoryMock.Setup(library => library.Create(_skillChange)).Throws<Exception>();

            Assert.Throws<Exception>(() => _skillChangeMediator.ChangeSkill(_skillChange));
        }
    }
}