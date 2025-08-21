namespace IdelPog.Console.Argument.Interface
{
    public interface IArgumentResolver<out T>
    {
        public T Resolve(string argument);
    }
}