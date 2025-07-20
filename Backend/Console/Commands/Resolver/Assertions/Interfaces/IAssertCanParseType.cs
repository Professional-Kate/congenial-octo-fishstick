namespace Console.Commands.Resolver.Assertions
{
    public interface IAssertCanParseType
    {
        public void Handle(bool canParse, string argument, Type typeContext);
    }
}