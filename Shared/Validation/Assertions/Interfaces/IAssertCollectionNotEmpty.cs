namespace IdelPog.Validation.Assertions
{
    public interface IAssertCollectionNotEmpty
    {
        public void Handle<T>(IReadOnlyList<T> collection);
        
        public void Handle<T>(ReadOnlySpan<T> collection);
        
    }
}