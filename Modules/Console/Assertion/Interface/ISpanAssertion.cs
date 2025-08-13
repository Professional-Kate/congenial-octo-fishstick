namespace IdelPog.Console.Assertion.Interface
{
    public interface ISpanAssertion
    {
        public void AssertNotEmpty<T>(ReadOnlySpan<T> span);
    }
}