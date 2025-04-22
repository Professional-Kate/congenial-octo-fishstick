namespace ContentHydrator.Assertions.Pipelines
{
    public interface IDirectoryAsserter
    {
        public void AssertDirectory(string directoryPath);

        public void AssertFiles(string[] files, string directoryPathContext);
    }
}