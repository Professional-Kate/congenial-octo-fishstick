namespace IdelPog.Console.Assertion.Interface
{
    public interface ITypeParseAssertion
    {
        public void AssertCanParse(bool canParse, string argument, Type targetType);
    }
}