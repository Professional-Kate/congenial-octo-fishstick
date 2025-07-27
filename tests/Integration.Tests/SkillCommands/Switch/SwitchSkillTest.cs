using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Messaging.Buffer;
using IdelPog.SimulationEngine.Skill;

namespace Integration.Tests.SkillCommands.Switch
{
    [TestFixture]
    public class SwitchSkillTest : ManagedBuffer
    {
        private SkillChangeDTOListener _listener;
        private ICurrentSkillSetter _currentSkillSetter;
        private ICurrentSkillProvider _currentSkillProvider;

        private SkillChange _skillChange;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _skillChange = new SkillChange { SkillID = SkillID.FARMING, ResourceID = ResourceID.STONE};

            CurrentSkillProvider currentSkillProvider = new();
            _currentSkillSetter = currentSkillProvider;
            _currentSkillProvider = currentSkillProvider;
        }

        [SetUp]
        public void Setup()
        {
            new SkillBootstrapper().Initialize(BufferMessenger, BufferManager, _currentSkillSetter);

            _listener = new SkillChangeDTOListener();
            ManagedSubscribe(_listener);
        }

        private void SendChangeSkillBuffer(SkillChange skillChange)
        {
            IBuffer<SkillChange> buffer = BufferManager.RequestBuffer<SkillChange>(new BufferRequest(1));
            buffer.Assign([skillChange]);
            buffer.MarkReady();
        }

        private void AssertListener(SkillChange skillChange)
        {
            Assert.Multiple(() =>
            {
                Assert.That(_listener.WasCalled, Is.True);
                Assert.That(_currentSkillProvider.GetCurrentSkill(), Is.EqualTo(skillChange.SkillID));
                Assert.That(_listener.SkillChangeDTO.SkillID, Is.EqualTo(skillChange.SkillID));
            });
        }

        [Test]
        public void Positive_SendChangeSkill_SwitchesSkill_DispatchesSkillChangeDTO()
        {
            Assert.DoesNotThrow(() => SendChangeSkillBuffer(_skillChange));
            AssertListener(_skillChange);
        }

        [Test]
        public void Positive_SendSequentialCommands_SwitchesSkill_DispatchesSkillChangeDTO()
        {
            Assert.DoesNotThrow(() => SendChangeSkillBuffer(_skillChange));
            AssertListener(_skillChange);

            SkillChange secondSkillChange = new() { SkillID = SkillID.MINING, ResourceID = ResourceID.STONE};
            Assert.DoesNotThrow(() => SendChangeSkillBuffer(secondSkillChange));
            AssertListener(secondSkillChange);
        }
    }
}