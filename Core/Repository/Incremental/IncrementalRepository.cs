using IdelPog.Core.Repository.Asserter;

namespace IdelPog.Core.Repository.Incremental
{
    public sealed class IncrementalRepository<T> : IIncrementalRepository<T>
    {
        private readonly IDictionary<byte, T> _dictionary;
        private readonly IRepositoryAsserter _repositoryAsserter;
        private byte _id;

        public IncrementalRepository(IDictionary<byte, T> dictionary, IRepositoryAsserter repositoryAsserter)
        {
            _dictionary = dictionary;
            _repositoryAsserter = repositoryAsserter;
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
    }
}