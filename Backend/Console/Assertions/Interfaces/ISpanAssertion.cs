namespace Console.Assertions
{
    public interface ISpanAssertion
    {
        public void AssertNotEmpty<T>(ReadOnlySpan<T> span);
    }
}