namespace Console.Assertions
{
    public interface IAssertSpanNotEmpty
    {
        public void Handle<T>(ReadOnlySpan<T> span);
    }
}