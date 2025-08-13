using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Messaging.Listener.Single;

namespace IdelPog.Integration.Tests.Console.Schedule
{
    public class ScheduleControlListener : ISingleListener<ScheduleControl>
    {
        public Type ListenerType => typeof(ScheduleControl);
        public bool WasCalled { get; private set; }
        public ScheduleControl ScheduleControl { get; private set; }

        public void Handle(ScheduleControl message)
        {
            WasCalled = true;
            ScheduleControl = message;
        }
    }
}