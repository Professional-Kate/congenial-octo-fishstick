namespace IdelPog.Validation.Assertions
{
    public interface IFoundAssertion
    {
        public void AssertFound<TKey>(TKey key, bool found);
    }
}