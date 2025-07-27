using IdelPog.Validation.Assertions;

namespace IdelPog.ContentHydrator.Assertions.Pipelines
{
    public class DirectoryAssertionPipeline : IDirectoryAssertionPipeline
    {
        private readonly IDirectoryAssertion _directoryAssertion;
        private readonly IObjectNullAssertion _objectNullAssertion;

        public DirectoryAssertionPipeline(IDirectoryAssertion directoryAssertion, IObjectNullAssertion objectNullAssertion)
        {
            _directoryAssertion = directoryAssertion;
            _objectNullAssertion = objectNullAssertion;
        }

        public void AssertDirectory(string directoryPath)
        {
            _objectNullAssertion.AssertNotNull(directoryPath, nameof(directoryPath));
            _directoryAssertion.AssertDirectoryIsFound(directoryPath);
        }

        public void AssertFiles(int itemCount, string directoryPath)
        {
            _objectNullAssertion.AssertNotNull(directoryPath, nameof(directoryPath));
            _directoryAssertion.AssertDirectoryNotEmpty(itemCount, directoryPath);
        }
    }
}