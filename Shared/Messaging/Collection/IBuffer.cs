namespace IdelPog.Messaging.Collection
{
    public interface IBuffer<in T>
    {
        /// <summary>
        /// Assign the passed array of data into the internal collection of the Buffer
        /// </summary>
        /// <param name="source">The data you want in the Buffer</param>
        public void Assign(T[] source);
        
        /// <summary>
        /// Mark this buffer ready for consuming. After marking ready you will no longer be able to <see cref="Assign"/>
        /// </summary>
        public void MarkReady();
    }
}