namespace IdelPog.Console.Resolver
{
    public interface IArgumentResolverPipeline<out T>
    {
        public T Resolve(ReadOnlySpan<string> arguments);
    }
}