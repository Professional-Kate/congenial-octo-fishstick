namespace Console.Assertions
{
    public interface IEnumParseAssertion
    {
        public void AssertCanParse(bool canParse, string argument, string enumName);
    }
}