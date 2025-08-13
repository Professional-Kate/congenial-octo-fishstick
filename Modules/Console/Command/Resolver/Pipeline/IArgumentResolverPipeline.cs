namespace IdelPog.Console.Command.Resolver.Pipeline
{
    public interface IArgumentResolverPipeline<out T>
    {
        public T Resolve(ReadOnlySpan<string> arguments);
    }
}