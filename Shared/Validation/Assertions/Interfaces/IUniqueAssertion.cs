namespace IdelPog.Validation.Assertions
{
    public interface IUniqueAssertion
    {
        public void AssertUnique<TKey>(TKey key, bool exists);
    }
}