namespace IdelPog.Validation.Assertions
{
    public interface IAssertFound
    {
        public void AssertItemIsFound(object key, Func<bool> itemNotFound);
    }
}