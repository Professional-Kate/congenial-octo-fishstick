namespace IdelPog.Core.Scheduler.Error.Factory
{
    public interface ITaskErrorFactory
    {
        public ScheduledTaskError Create(Exception exception, Type taskType);
    }
}