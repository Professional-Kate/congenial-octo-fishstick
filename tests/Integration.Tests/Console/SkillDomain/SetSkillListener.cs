using IdelPog.Common.Commands;
using IdelPog.Messaging.Listeners;

namespace Integration.Tests.Console
{
    public class SetSkillListener : ISingleListener<SetSkill>
    {
        public Type ListenerType => typeof(SetSkill);
        public bool WasCalled { get; private set; }
        public SetSkill SetSkill { get; private set; }

        public void Handle(SetSkill message)
        {
            WasCalled = true;
            SetSkill = message;
        }
    }
}