namespace ContentHydrator.Service
{
    public interface IHydrator
    {
        public IEnumerable<T> HydrateFrom<T>(string sourceDirectory);
    }
}