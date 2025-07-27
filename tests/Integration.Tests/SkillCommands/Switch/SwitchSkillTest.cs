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

        private SetSkill _setSkill;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _setSkill = new SetSkill { SkillID = SkillID.FARMING };

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

        private void SendChangeSkillBuffer(SetSkill setSkill)
        {
            IBuffer<SetSkill> buffer = BufferManager.RequestBuffer<SetSkill>(new BufferRequest(1));
            buffer.Assign([setSkill]);
            buffer.MarkReady();
        }

        private void AssertListener(SetSkill setSkill)
        {
            Assert.Multiple(() =>
            {
                Assert.That(_listener.WasCalled, Is.True);
                Assert.That(_currentSkillProvider.GetCurrentSkill(), Is.EqualTo(setSkill.SkillID));
                Assert.That(_listener.SetSkillDTO.SkillID, Is.EqualTo(setSkill.SkillID));
            });
        }

        [Test]
        public void Positive_SendChangeSkill_SwitchesSkill_DispatchesSkillChangeDTO()
        {
            Assert.DoesNotThrow(() => SendChangeSkillBuffer(_setSkill));
            AssertListener(_setSkill);
        }

        [Test]
        public void Positive_SendSequentialCommands_SwitchesSkill_DispatchesSkillChangeDTO()
        {
            Assert.DoesNotThrow(() => SendChangeSkillBuffer(_setSkill));
            AssertListener(_setSkill);

            SetSkill secondSetSkill = new() { SkillID = SkillID.MINING };
            Assert.DoesNotThrow(() => SendChangeSkillBuffer(secondSetSkill));
            AssertListener(secondSetSkill);
        }
    }
}