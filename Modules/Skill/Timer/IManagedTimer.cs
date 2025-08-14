namespace IdelPog.Skill.Timer
{
    public interface IManagedTimer : IDisposable
    {
        public void Start(TimeSpan dueTime, TimeSpan period);

        public void Stop();
    }
}