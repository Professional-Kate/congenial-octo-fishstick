using IdelPog.Validation.Assertions.Interfaces;

namespace IdelPog.SimulationEngine.Service
{
    public class Mapper<T>(IAssertFound assertFound, IAssertNonDuplicate assertUnique) : IMapper<T>
    {
        private readonly Dictionary<T, Structures.Types.Information> _information = new();

        public Structures.Types.Information GetInformation(T key)
        {
            bool contains = _information.TryGetValue(key, out Structures.Types.Information information);
            assertFound.AssertItemIsFound(key, () => contains == false);
            
            return information;
        }

        public void AddInformation(T key, Structures.Types.Information information)
        {
            assertUnique.AssertContains(key, () => _information.ContainsKey(key));
            
            _information.Add(key, information);
        }
    }
}