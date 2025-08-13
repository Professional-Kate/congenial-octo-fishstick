namespace IdelPog.Console.Assertion.Interface
{
    public interface INumberAssertion
    {
        public void AssertNonNegative(int number);

        public void AssertAllNonNegative(int[] numbers);
    }
}