using IdelPog.Common.Commands;
using IdelPog.Messaging.Listeners;

namespace Integration.Tests.SkillCommands.Switch
{
    public class SetSkillListener : ISingleListener<SetSkill>
    {
        public Type ListenerType { get; } = typeof(SetSkill);
        public SetSkill SetSkill { get; private set; }
        public bool WasCalled { get; private set; }

        public void Handle(SetSkill message)
        {
            SetSkill = message;
            WasCalled = true;
        }
    }
}