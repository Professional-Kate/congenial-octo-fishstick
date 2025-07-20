namespace Console.Commands.Resolver.Assertions
{
    public interface IAssertArgumentLength
    {
        public void Handle(int actualSize, int expectedSize);
    }
}