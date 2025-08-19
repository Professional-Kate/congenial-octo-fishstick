using IdelPog.Core.Messaging.Dispatcher.Single;

namespace IdelPog.Core.Scheduler.Runner
{
    public sealed class TickEmitter : ITickEmitter
    {
        private readonly IDispatchOne<ScheduleTick> _tickDispatcher;

        public TickEmitter(IDispatchOne<ScheduleTick> tickDispatcher)
        {
            _tickDispatcher = tickDispatcher;
        }

        public void RunUpdate()
        { 
            _tickDispatcher.Dispatch(new  ScheduleTick());
        }
    }
}