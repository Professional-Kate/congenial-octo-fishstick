namespace IdelPog.Content.Hydrator.Assertion.Pipeline
{
    public interface IDirectoryAssertionPipeline
    {
        public void AssertDirectory(string directoryPath);

        public void AssertFiles(int itemCount, string directoryPath);
    }
}