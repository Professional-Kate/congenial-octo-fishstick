using IdelPog.SimulationEngine.Skill;
using Moq;

namespace IdelPogTests.Controller
{
    [TestFixture]
    public class SkillControllerTest
    {
        private ISkillController _controller { get; set; }
        private Mock<ICurrentSkillSetter> _currentSkillSetterMock { get; set; }
        private SkillChange _skillChange { get; set; }

        [SetUp]
        public void SetUp()
        {
            _skillChange = new SkillChange { SkillID = SkillID.MINING };
            _currentSkillSetterMock = new Mock<ICurrentSkillSetter>();
            _controller = new SkillController(_currentSkillSetterMock.Object);
        }

        [Test]
        public void Positive_SwitchSkill_InvokesSkillSetter()
        {
            _controller.SwitchSkill(_skillChange);
            
            _currentSkillSetterMock.Verify(library => library.SetCurrentSkill(_skillChange.SkillID), Times.Once());
        }
    }
}