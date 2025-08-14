namespace IdelPog.Console.Command.Resolver
{
    public interface IArgumentResolver<out T>
    {
        public T Resolve(string argument);
    }
}