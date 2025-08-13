namespace IdelPog.Core.Validation.Assertion.Interface
{
    public interface IFoundAssertion
    {
        public void AssertFound<TKey>(TKey key, bool found);
    }
}