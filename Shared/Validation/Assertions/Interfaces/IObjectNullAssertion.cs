namespace IdelPog.Validation.Assertions
{
    public interface IObjectNullAssertion
    {
        public void AssertNotNull<T>(T? value, string paramName);
    }
}