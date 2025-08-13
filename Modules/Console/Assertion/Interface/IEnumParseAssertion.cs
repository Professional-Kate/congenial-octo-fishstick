namespace IdelPog.Console.Assertion.Interface
{
    public interface IEnumParseAssertion
    {
        public void AssertCanParse(bool canParse, string argument, string enumName);
    }
}