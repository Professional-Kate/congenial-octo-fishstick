using IdelPog.Common.Commands;
using IdelPog.Messaging.Listeners;

namespace Integration.Tests.Console
{
    public class ScheduleControlListener : ISingleListener<ScheduleControl>
    {
        public Type ListenerType => typeof(ScheduleControl);
        public bool WasCalled { get; private set; }
        public ScheduleControl ScheduleControl { get; private set; }

        public void Handle(ScheduleControl harvestNode)
        {
            WasCalled = true;
            ScheduleControl = harvestNode;
        }
    }
}