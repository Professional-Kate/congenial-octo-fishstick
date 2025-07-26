namespace IdelPog.ContentHydrator.Assertions.Pipelines
{
    public interface IDirectoryAssertionPipeline
    {
        public void AssertDirectory(string directoryPath);

        public void AssertFiles(int itemCount, string directoryPathContext);
    }
}