namespace IdelPog.Core.Validation.Assertion.Interface
{
    public interface IObjectNullAssertion
    {
        public void AssertNotNull<T>(T? value, string paramName);
    }
}