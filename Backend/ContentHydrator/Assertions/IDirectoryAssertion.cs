namespace IdelPog.ContentHydrator.Assertions
{
    public interface IDirectoryAssertion
    {
        public void AssertDirectoryIsFound(string path);

        public void AssertDirectoryNotEmpty(int itemCount, string path);
    }
}