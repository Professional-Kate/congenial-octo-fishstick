using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.SimulationEngine.Skill;
using Moq;

namespace IdelPogTests.Controller
{
    [TestFixture]
    public class SkillControllerTest
    {
        private ISkillController _controller { get; set; }
        private Mock<ISkillChangeMediator> _skillChangeMediatorMock { get; set; }
        private SetSkill _setSkill { get; set; }

        [SetUp]
        public void SetUp()
        {
            _setSkill = new SetSkill { SkillID = SkillID.MINING };
            _skillChangeMediatorMock = new Mock<ISkillChangeMediator>();
            _controller = new SkillController(_skillChangeMediatorMock.Object);
        }

        [Test]
        public void Positive_SwitchSkill_InvokesMediator()
        {
            _controller.ChangeSkill(_setSkill);

            _skillChangeMediatorMock.Verify(library => library.ChangeSkill(_setSkill), Times.Once());
        }

        [Test]
        public void Positive_ChangeSkill_NoExceptionSuppression()
        {
            _skillChangeMediatorMock.Setup(library => library.ChangeSkill(_setSkill))
                .Throws<Exception>();

            Assert.Throws<Exception>(() => _controller.ChangeSkill(_setSkill));
        }
    }
}