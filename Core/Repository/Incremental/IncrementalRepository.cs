using IdelPog.Core.Repository.Asserter;

namespace IdelPog.Core.Repository.Incremental
{
    public sealed class IncrementalRepository<T> : IIncrementalRepository<T>
    {
        private readonly IDictionary<byte, T> _dictionary;
        private readonly IRepositoryAsserter _repositoryAsserter;
        private byte _id;

        public IncrementalRepository(IRepositoryAsserter repositoryAsserter)
        {
            _repositoryAsserter = repositoryAsserter;
            _dictionary = new Dictionary<byte, T>();
        }

        public byte Add(T value)
        {
            _repositoryAsserter.AssertNotNull(value);

            byte id = _id;
            checked
            {
                _id++;
            }
            
            _dictionary.Add(id, value);
            
            return id;
        }

        public bool Contains(byte id)
        { 
            return _dictionary.ContainsKey(id);
        }

        public T Get(byte id)
        {
            _repositoryAsserter.AssertFound(id, _dictionary.ContainsKey(id));
            return _dictionary[id];
        }

        public IEnumerable<T> Enumerate() => _dictionary.Values;

        public byte GetID()
        {
            return _id;
        }
    }
}