namespace Console.Commands.Resolver
{
    public interface IArgumentResolver<out T>
    {
        public T Resolve(string argument);
    }
}