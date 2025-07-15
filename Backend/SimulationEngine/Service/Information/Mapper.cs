using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.SimulationEngine.Service
{
    public class Mapper<T> : IMapper<T>
    {
        private readonly Dictionary<T, Structures.Types.Information> _information = new();
        
        private readonly IAssertFound _assertFound;
        private readonly IAssertNonDuplicate _assertNonDuplicate;

        public Mapper()
        {
            _assertFound = new AssertFound(new ThrowHandler());
            _assertNonDuplicate = new AssertNonDuplicate(new ThrowHandler());
        }

        public Mapper(IAssertFound assertFound, IAssertNonDuplicate assertUnique)
        {
            _assertFound = assertFound;
            _assertNonDuplicate = assertUnique;
        }

        public Structures.Types.Information GetInformation(T key)
        {
            bool contains = _information.TryGetValue(key, out Structures.Types.Information information);
            _assertFound.AssertItemIsFound(key, () => contains == false);
            
            return information;
        }

        public void AddInformation(T key, Structures.Types.Information information)
        {
            _assertNonDuplicate.AssertContains(key, () => _information.ContainsKey(key));
            
            _information.Add(key, information);
        }
    }
}