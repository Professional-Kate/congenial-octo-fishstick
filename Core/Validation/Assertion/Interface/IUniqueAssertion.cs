namespace IdelPog.Core.Validation.Assertion.Interface
{
    public interface IUniqueAssertion
    {
        public void AssertUnique<TKey>(TKey key, bool exists);
    }
}