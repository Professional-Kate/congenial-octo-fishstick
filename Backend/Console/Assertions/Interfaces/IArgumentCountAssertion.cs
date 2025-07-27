namespace Console.Assertions
{
    public interface IArgumentCountAssertion
    {
        public void AssertCount(int actualCount, int expectedCount);
    }
}