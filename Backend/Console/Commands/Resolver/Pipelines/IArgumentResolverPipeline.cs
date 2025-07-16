namespace Console.Commands.Resolver.Pipelines
{
    public interface IArgumentResolverPipeline<out T>
    {
        public T Resolve(ReadOnlySpan<string> arguments);
    }
}