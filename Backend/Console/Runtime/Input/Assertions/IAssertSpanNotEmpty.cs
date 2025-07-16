namespace Console.Runtime.Input.Assertions
{
    public interface IAssertSpanNotEmpty
    {
        public void Handle<T>(ReadOnlySpan<T> span);
    }
}