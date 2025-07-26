using IdelPog.Validation.Assertions;

namespace IdelPog.ContentHydrator.Assertions.Pipelines
{
    public class DirectoryAssertionPipeline : IDirectoryAssertionPipeline
    {
        private readonly IDirectoryAssertion _directoryAssertion;
        private readonly IAssertNotNull _assertNotNull;

        public DirectoryAssertionPipeline(IDirectoryAssertion directoryAssertion, IAssertNotNull assertNotNull)
        {
            _directoryAssertion = directoryAssertion;
            _assertNotNull = assertNotNull;
        }

        public void AssertDirectory(string directoryPath)
        {
            _assertNotNull.AssertObjectNotNull(directoryPath);
            _directoryAssertion.AssertDirectoryIsFound(directoryPath);
        }

        public void AssertFiles(int itemCount, string directoryPathContext)
        {
            _assertNotNull.AssertObjectNotNull(directoryPathContext);
            _directoryAssertion.AssertDirectoryNotEmpty(itemCount, directoryPathContext);
        }
    }
}