using IdelPog.Validation.Assertions;

namespace ContentHydrator.Assertions.Pipelines
{
    public class DirectoryAsserter(IAssertFound assertFound, IAssertDirectoryNotEmpty notEmpty) : IDirectoryAsserter
    {
        public void AssertDirectory(string directoryPath)
        {
            assertFound.AssertItemIsFound(directoryPath, () => Directory.Exists(directoryPath));
        }

        public void AssertFiles(string[] files, string directoryPath)
        {
            notEmpty.AssertNotEmpty(files, directoryPath);
        }
    }
}