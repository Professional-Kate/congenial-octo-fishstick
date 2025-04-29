namespace IdelPog.Staging.Collection
{
    public interface IBuffer<in T>
    {
        public void MarkReady();
        public void Assign(T[] source);
    }
}