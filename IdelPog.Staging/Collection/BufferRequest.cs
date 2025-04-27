namespace IdelPog.Staging.Collection
{
    public sealed record BufferRequest<T>(int Length)
    {
        public readonly Type DataType = typeof(T);
    }
}