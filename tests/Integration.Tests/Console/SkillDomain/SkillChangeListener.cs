using IdelPog.Common.Commands;
using IdelPog.Messaging.Listeners;

namespace Integration.Tests.Console
{
    public class SkillChangeListener : ISingleListener<SetSkill>
    {
        public Type ListenerType => typeof(SetSkill);
        public bool WasCalled { get; private set; }
        public SetSkill SetSkill { get; private set; }

        public void Handle(SetSkill harvestNode)
        {
            WasCalled = true;
            SetSkill = harvestNode;
        }
    }
}