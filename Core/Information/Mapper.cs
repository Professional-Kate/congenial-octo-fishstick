using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Core.Information
{
    public sealed class Mapper<T> : IMapper<T> where T : notnull
    {
        private readonly Dictionary<T, Contracts.Information> _information = new();

        private readonly IFoundAssertion _foundAssertion;
        private readonly IUniqueAssertion _uniqueAssertion;

        public Mapper(IFoundAssertion foundAssertion, IUniqueAssertion unique)
        {
            _foundAssertion = foundAssertion;
            _uniqueAssertion = unique;
        }

        public Contracts.Information GetInformation(T key)
        {
            bool contains = _information.TryGetValue(key, out Contracts.Information information);
            _foundAssertion.AssertFound(key, contains);

            return information;
        }

        public void AddInformation(T key, Contracts.Information information)
        {
            _uniqueAssertion.AssertUnique(key, _information.ContainsKey(key));

            _information.Add(key, information);
        }
    }
}