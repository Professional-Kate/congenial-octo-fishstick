using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Messaging.Controller;
using IdelPog.Messaging.Listeners.Single;
using Moq;

namespace IdelPogTests.Controller
{
    [TestFixture]
    public class SkillControllerTest
    {
        private ISingleController<SetSkill> _controller { get; set; }
        private Mock<ISingleMediator<SetSkill>> _skillChangeMediatorMock { get; set; }
        private SetSkill _setSkill { get; set; }

        [SetUp]
        public void SetUp()
        {
            _setSkill = new SetSkill { SkillID = SkillID.MINING };
            _skillChangeMediatorMock = new Mock<ISingleMediator<SetSkill>>();
            _controller = new ManagedSingleController<SetSkill>(_skillChangeMediatorMock.Object);
        }

        [Test]
        public void Positive_SwitchSkill_InvokesMediator()
        {
            _controller.HandleMessage(_setSkill);

            _skillChangeMediatorMock.Verify(library => library.HandleMessage(_setSkill), Times.Once());
        }

        [Test]
        public void Positive_ChangeSkill_NoExceptionSuppression()
        {
            _skillChangeMediatorMock.Setup(library => library.HandleMessage(_setSkill))
                .Throws<Exception>();

            Assert.Throws<Exception>(() => _controller.HandleMessage(_setSkill));
        }
    }
}