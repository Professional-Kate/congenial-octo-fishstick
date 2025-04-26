namespace IdelPog.Buffer.Collection
{
    public sealed record BufferRequest<T>(int Length)
    {
        public readonly Type DataType = typeof(T);
    }
}