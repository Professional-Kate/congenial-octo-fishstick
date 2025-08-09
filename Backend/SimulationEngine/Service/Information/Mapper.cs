using IdelPog.Common.Structures;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.SimulationEngine.Service
{
    public class Mapper<T> : IMapper<T> where T : notnull
    {
        private readonly Dictionary<T, Information> _information = new();

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

        public Information GetInformation(T key)
        {
            bool contains = _information.TryGetValue(key, out Information information);
            _foundAssertion.AssertFound(key, contains);

            return information;
        }

        public void AddInformation(T key, Information information)
        {
            _uniqueAssertion.AssertUnique(key, _information.ContainsKey(key));

            _information.Add(key, information);
        }
    }
}