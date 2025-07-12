using IdelPog.SimulationEngine.Skill;

namespace Integration.Tests.SkillCommands.Switch
{
    [TestFixture]
    public class SwitchSkillTest : ManagedBuffer
    {
        private SkillChangeDTOListener _listener;
        private ICurrentSkillSetter _currentSkillSetter;
        private ICurrentSkillProvider _currentSkillProvider;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            CurrentSkillProvider currentSkillProvider = new();
            _currentSkillSetter = currentSkillProvider;
            _currentSkillProvider = currentSkillProvider;
            SkillBootstrapper.Initialize(BufferMessenger, _currentSkillSetter);
        }
        
        [SetUp]
        public void Setup()
        {
            _listener = new SkillChangeDTOListener();
        }
    }
}