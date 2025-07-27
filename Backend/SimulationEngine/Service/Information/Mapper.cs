using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.SimulationEngine.Service
{
    public class Mapper<T> : IMapper<T>
    {
        private readonly Dictionary<T, Structures.Types.Information> _information = new();

        private readonly IFoundAssertion _foundAssertion;
        private readonly IUniqueAssertion _uniqueAssertion;

        public Mapper()
        {
            _foundAssertion = new FoundAssertion(new ThrowHandler());
            _uniqueAssertion = new UniqueAssertion(new ThrowHandler());
        }

        public Mapper(IFoundAssertion foundAssertion, IUniqueAssertion unique)
        {
            _foundAssertion = foundAssertion;
            _uniqueAssertion = unique;
        }

        public Structures.Types.Information GetInformation(T key)
        {
            bool contains = _information.TryGetValue(key, out Structures.Types.Information information);
            _foundAssertion.AssertFound(key, contains);

            return information;
        }

        public void AddInformation(T key, Structures.Types.Information information)
        {
            _uniqueAssertion.AssertUnique(key, _information.ContainsKey(key));

            _information.Add(key, information);
        }
    }
}