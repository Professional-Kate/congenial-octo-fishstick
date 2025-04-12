using IdelPog.Engine.Validation.Assertions.Interfaces;

namespace IdelPog.Engine.Service.Information
{
    public class Mapper<T> : IMapper<T>
    {
        private readonly Dictionary<T, Structures.Information> _information = new();
        private readonly IAssertFound _assertFound;
        private readonly IAssertNonDuplicate _assertUnique;

        public Mapper(IAssertFound assertFound, IAssertNonDuplicate assertUnique)
        {
            _assertFound = assertFound;
            _assertUnique = assertUnique;
        }
        
        public Structures.Information GetInformation(T key)
        {
            bool contains = _information.TryGetValue(key, out Structures.Information information);
            _assertFound.AssertItemIsFound(key, () => contains == false);
            
            return information;
        }

        public void AddInformation(T key, Structures.Information information)
        {
            _assertUnique.AssertContains(key, () => _information.ContainsKey(key));
            
            _information.Add(key, information);
        }
    }
}