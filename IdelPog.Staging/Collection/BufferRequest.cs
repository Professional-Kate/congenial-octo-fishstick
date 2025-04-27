namespace IdelPog.Staging.Collection
{
    public sealed record BufferRequest<T>(int Length)
    {
        public Type DataType => typeof(T);
    }
}