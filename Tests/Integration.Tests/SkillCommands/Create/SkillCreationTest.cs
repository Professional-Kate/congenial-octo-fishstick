using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Information.Contracts;
using IdelPog.Core.Messaging.Buffer;

namespace IdelPog.Integration.Tests.SkillCommands.Create
{
    [TestFixture]
    public sealed class SkillCreationTest : ManagedTestBuffer
    {
        private SkillCreationErrorListener _skillCreationErrorListener;
        private SkillCreationResponseListener _skillCreationResponseListener;

        private SkillCreation _miningCreation;
        private SkillCreation _woodCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _miningCreation = new SkillCreation
            {
                Information = new Information { Name = "", Description = "" },
                ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, ExperiencePerAction = 0, Level = 0, NextLevelExperience = 0 },
                SkillID = SkillID.MINING
            };
            
            _woodCreation = new SkillCreation
            {
                Information = new Information { Name = "", Description = "" },
                ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, ExperiencePerAction = 0, Level = 0, NextLevelExperience = 0 },
                SkillID = SkillID.WOOD_CUTTING
            };
        }

        [SetUp]
        public void Setup()
        {
            _skillCreationErrorListener = new SkillCreationErrorListener();
            _skillCreationResponseListener = new SkillCreationResponseListener();
            
            ManagedSubscribe(_skillCreationErrorListener);
            ManagedSubscribe(_skillCreationResponseListener);
        }

        private void SendSkillCreationBuffer(SkillCreation[] skillCreations)
        {
            IBuffer<SkillCreation> buffer = BufferManager.RequestBuffer<SkillCreation>(new BufferRequest(skillCreations.Length));
            buffer.Assign(skillCreations);
            buffer.MarkReady();
        }

        [Test]
        public void Positive_SendSingleCommand_CreatesSkill_DispatchesResponse()
        {
            Assert.DoesNotThrow(() => SendSkillCreationBuffer([_miningCreation]));
        }
    }
}