namespace ContentHydrator.Orchestration
{
    public interface IHydrationOrchestrator
    {
        public void HydrateTo<T>(string sourceDirectory);
    }
}