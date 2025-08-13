using IdelPog.Content.Hydrator.Exceptions;
using IdelPog.Core.Validation;
using IdelPog.Core.Validation.Handler.Interface;

namespace IdelPog.Content.Hydrator.Assertion
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