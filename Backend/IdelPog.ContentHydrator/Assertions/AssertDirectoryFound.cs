using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace ContentHydrator.Assertions
{
    public class AssertDirectoryFound(IHandler handler) : BaseAssertion<DirectoryNotFoundException>(handler), IAssertDirectoryFound
    {
        public void AssertDirectoryIsFound(string directoryPath)
        {
            Assert(() =>
            {
                if (Directory.Exists(directoryPath) == false)
                {
                    throw new DirectoryNotFoundException(directoryPath);
                }
            });
        }
    }
}