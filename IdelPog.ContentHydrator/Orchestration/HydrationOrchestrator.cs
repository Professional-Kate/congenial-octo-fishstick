using ContentHydrator.Providers;
using ContentHydrator.Readers;

namespace ContentHydrator.Orchestration
{
    public class HydrationOrchestrator(IJsonReader reader, IConverterProvider provider) : IHydrationOrchestrator
    {
        public void HydrateTo<T>(string sourceDirectory)
        {
            throw new NotImplementedException();
        }
    }
}