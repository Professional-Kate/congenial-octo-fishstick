using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Buffer;

namespace IdelPog.Integration.Tests.SkillCommands.Switch
{
    [TestFixture]
    public class SetSkillTest : ManagedBuffer
    {
        private SetSkillListener _listener;
        private SetSkill _setSkill;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _setSkill = new SetSkill { SkillID = SkillID.FARMING };
        }

        [SetUp]
        public void Setup()
        {
            _listener = new SetSkillListener();
            ManagedSubscribe(_listener);
        }

        private void SendSetSkillBuffer(SetSkill setSkill)
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
                Assert.That(_listener.SetSkill.SkillID, Is.EqualTo(setSkill.SkillID));
            });
        }

        [Test]
        public void Positive_SendChangeSkill_SwitchesSkill_DispatchesSkillChangeDTO()
        {
            Assert.DoesNotThrow(() => SendSetSkillBuffer(_setSkill));
            AssertListener(_setSkill);
        }

        [Test]
        public void Positive_SendSequentialCommands_SwitchesSkill_DispatchesSkillChangeDTO()
        {
            Assert.DoesNotThrow(() => SendSetSkillBuffer(_setSkill));
            AssertListener(_setSkill);

            SetSkill secondSkillChange = new() { SkillID = SkillID.MINING };
            Assert.DoesNotThrow(() => SendSetSkillBuffer(secondSkillChange));
            AssertListener(secondSkillChange);
        }
    }
}