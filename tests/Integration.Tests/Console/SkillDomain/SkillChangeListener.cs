using IdelPog.Common.Commands;
using IdelPog.Messaging.Listeners;
using IdelPog.Messaging.Listeners.Buffer;

namespace Integration.Tests.Console
{
    public class SkillChangeListener : ISingleListener<SkillChange>
    {
        public Type ListenerType => typeof(SkillChange);
        public bool WasCalled { get; private set; }
        public SkillChange SkillChange { get; private set; }

        public void Handle(SkillChange item)
        {
            WasCalled = true;
            SkillChange = item;
        }
    }
}