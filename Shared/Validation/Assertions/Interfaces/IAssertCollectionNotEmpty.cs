namespace IdelPog.Validation.Assertions.Interfaces
{
    public interface IAssertCollectionNotEmpty
    {
        public void Handle<T>(IReadOnlyList<T> collection);
    }
}