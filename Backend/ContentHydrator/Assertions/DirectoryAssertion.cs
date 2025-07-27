using IdelPog.ContentHydrator.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.ContentHydrator.Assertions
{
    public class DirectoryAssertion : BaseAssertion, IDirectoryAssertion
    {
        public DirectoryAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertDirectoryIsFound(string path)
        {
            Assert<DirectoryNotFoundException>(() =>
            {
                if (Directory.Exists(path) == false)
                {
                    throw new DirectoryNotFoundException(path);
                }
            });
        }

        public void AssertDirectoryNotEmpty(int itemCount, string path)
        {
            Assert<EmptyDirectoryException>(() =>
            {
                if (itemCount <= 0)
                {
                    throw new EmptyDirectoryException(path);
                }
            });
        }
    }
}