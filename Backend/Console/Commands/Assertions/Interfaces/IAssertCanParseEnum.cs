namespace Console.Commands.Assertions
{
    public interface IAssertCanParseEnum
    {
        public void Handle(bool canParse, string argument, string enumName);
    }
}