namespace IdelPog.Core.Repository.Incremental
{
    public interface IIncrementalRepository<T>
    {
        public byte Add(T value);

        public bool Contains(byte id);

        public T Get(byte id);
        
        public IEnumerable<T> Enumerate();

        public byte GetID();
    }
}