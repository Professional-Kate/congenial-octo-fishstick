using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
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
        private Mock<ISkillChangeFactory> _skillChangeFactoryMock;
        private Mock<IDispatchOne<SkillChangeDTO>> _skillChangeDispatcherMock;

        private SetSkill _setSkill;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _setSkill = new SetSkill { SkillID = SkillID.MINING };

            _currentSkillSetterMock = new Mock<ICurrentSkillSetter>();
            _skillChangeFactoryMock = new Mock<ISkillChangeFactory>();
            _skillChangeDispatcherMock = new Mock<IDispatchOne<SkillChangeDTO>>();

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
            SkillChangeDTO dto = new() { SkillID = _setSkill.SkillID };
            _skillChangeFactoryMock.Setup(library => library.CreateSkillChangeDTO(_setSkill)).Returns(dto);

            Assert.DoesNotThrow(() => _skillChangeMediator.ChangeSkill(_setSkill));

            _currentSkillSetterMock.Verify(library => library.SetCurrentSkill(_setSkill.SkillID), Times.Once);
            _skillChangeFactoryMock.Verify(library => library.CreateSkillChangeDTO(_setSkill), Times.Once);
            _skillChangeDispatcherMock.Verify(library => library.Dispatch(dto), Times.Once);
        }

        [Test]
        public void Positive_ChangeSkill_DoesNotSuppressExceptions()
        {
            _skillChangeFactoryMock.Setup(library => library.CreateSkillChangeDTO(_setSkill)).Throws<Exception>();

            Assert.Throws<Exception>(() => _skillChangeMediator.ChangeSkill(_setSkill));
        }
    }
}