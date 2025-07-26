namespace IdelPog.ContentHydrator.Assertions.Pipelines
{
    public interface IDirectoryPipelineAssertion
    {
        public void AssertDirectory(string directoryPath);

        public void AssertFiles(int itemCount, string directoryPathContext);
    }
}