namespace IdelPog.Content.Hydrator.Assertion
{
    public interface IDirectoryAssertion
    {
        public void AssertDirectoryIsFound(string path);

        public void AssertDirectoryNotEmpty(int itemCount, string path);
    }
}