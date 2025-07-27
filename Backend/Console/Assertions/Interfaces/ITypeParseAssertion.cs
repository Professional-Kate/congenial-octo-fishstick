namespace Console.Assertions
{
    public interface ITypeParseAssertion
    {
        public void AssertCanParse(bool canParse, string argument, Type targetType);
    }
}