namespace Console.Commands.Resolver.Pipelines
{
    public interface IArgumentResolverPipeline<out T>
    {
        public T Resolve(string[] arguments);
    }
}