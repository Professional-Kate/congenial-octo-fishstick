using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Information.Contracts;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Messaging.Buffer.Manager;
using IdelPog.Core.Progression;
using IdelPog.Skill.Contracts.Command;

namespace IdelPog.Integration.Tests.SkillCommands
{
    internal sealed class SkillCreationDispatcher
    {
        public readonly SkillCreation MiningCreation = new()
        {
            Information = new Information { Name = "", Description = "" },
            ReadOnlyLevelable = new ReadOnlyLevelable { Experience = 0, ExperiencePerAction = 25, Level = 1, NextLevelExperience = 332 },
            SkillID = SkillID.MINING
        };

        public static void SendSkillCreationBuffer(SkillCreation[] skillCreations, IBufferManager bufferManager)
        {
            IBuffer<SkillCreation> buffer = bufferManager.RequestBuffer<SkillCreation>(new BufferRequest(skillCreations.Length));
            buffer.Assign(skillCreations);
            buffer.MarkReady();
        }
    }
}