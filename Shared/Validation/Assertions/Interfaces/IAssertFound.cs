namespace IdelPog.Validation.Assertions.Interfaces
{
    public interface IAssertFound
    {
        public void AssertItemIsFound(object key, Func<bool> itemNotFound);
    }
}