using IdelPog.Validation.Assertions;

namespace IdelPog.ContentHydrator.Assertions.Pipelines
{
    public class DirectoryAsserter(IAssertDirectoryFound assertFound, IAssertDirectoryNotEmpty notEmpty, IAssertNotNull assertNotNull) : IDirectoryAsserter
    {
        public void AssertDirectory(string directoryPath)
        {
            assertNotNull.AssertObjectNotNull(directoryPath);
            assertFound.AssertDirectoryIsFound(directoryPath);
        }

        public void AssertFiles(string[] files, string directoryPathContext)
        {
            assertNotNull.AssertObjectNotNull(files);
            notEmpty.AssertNotEmpty(files, directoryPathContext);
        }
    }
}