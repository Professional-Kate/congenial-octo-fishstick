namespace IdelPog.Common.Repository
{
    /// <summary>
    /// Generic asset repository 
    /// </summary>
    /// <typeparam name="TID">The key used to identify and access the value</typeparam>
    /// <typeparam name="T">The type stored in the repository</typeparam>
    public interface IAssetRepository<in TID, T>
    {
        /// <inheritdoc cref="IStateRepository{TID,T}.Add"/>
        public void Add(TID key, T value);
        
        /// <inheritdoc cref="IStateRepository{TID,T}.Remove"/>
        public void Remove(TID key);
        
        /// <inheritdoc cref="IStateRepository{TID,T}.Get"/>
        /// <remarks>
        /// The objects are not expected to be mutated
        /// </remarks>
        public T Get(TID key);
        
        /// <inheritdoc cref="IStateRepository{TID,T}.Contains"/>
        public bool Contains(TID key);
    }
}