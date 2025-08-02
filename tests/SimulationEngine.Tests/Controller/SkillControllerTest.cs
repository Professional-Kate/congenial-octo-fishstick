using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Messaging.Controller;
using IdelPog.Messaging.Listeners.Single;
using IdelPog.SimulationEngine.Skill;
using Moq;

namespace IdelPogTests.Controller
{
    [TestFixture]
    public class SkillControllerTest
    {
        private ISingleController<SkillChange> _controller { get; set; }
        private Mock<ISingleMediator<SkillChange>> _skillChangeMediatorMock { get; set; }
        private SkillChange _skillChange { get; set; }

        [SetUp]
        public void SetUp()
        {
            _skillChange = new SkillChange { SkillID = SkillID.MINING };
            _skillChangeMediatorMock = new Mock<ISingleMediator<SkillChange>>();
            _controller = new ManagedSingleController<SkillChange>(_skillChangeMediatorMock.Object);
        }

        [Test]
        public void Positive_SwitchSkill_InvokesMediator()
        {
            _controller.HandleMessage(_skillChange);

            _skillChangeMediatorMock.Verify(library => library.HandleMessage(_skillChange), Times.Once());
        }

        [Test]
        public void Positive_ChangeSkill_NoExceptionSuppression()
        {
            _skillChangeMediatorMock.Setup(library => library.HandleMessage(_skillChange))
                .Throws<Exception>();

            Assert.Throws<Exception>(() => _controller.HandleMessage(_skillChange));
        }
    }
}