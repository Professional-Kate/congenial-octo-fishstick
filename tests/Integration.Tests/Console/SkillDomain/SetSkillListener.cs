using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Messaging.Listener.Single;

namespace IdelPog.Integration.Tests.Console.SkillDomain
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