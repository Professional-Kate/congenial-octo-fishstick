using IdelPog.Common.Commands;
using IdelPog.Messaging.Listeners;

namespace Integration.Tests.SkillCommands.Switch
{
    public class SkillChangeDTOListener : ISingleListener<SetSkill>
    {
        public Type ListenerType { get; } = typeof(SetSkill);
        public SetSkill SetSkillDTO { get; private set; }
        public bool WasCalled { get; private set; }

        public void Handle(SetSkill harvestNode)
        {
            SetSkillDTO = harvestNode;
            WasCalled = true;
        }
    }
}