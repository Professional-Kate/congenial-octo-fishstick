namespace Console.Commands.Assertions
{
    public interface IAssertArgumentLength
    {
        public void Handle(int actualSize, int expectedSize);
    }
}