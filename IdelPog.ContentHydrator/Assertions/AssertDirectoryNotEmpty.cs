using ContentHydrator.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace ContentHydrator.Assertions
{
    public class AssertDirectoryNotEmpty(IHandler handler) : BaseAssertion<EmptyDirectoryException>(handler), IAssertDirectoryNotEmpty
    {
        public void AssertNotEmpty(string[] items, string directoryPath)
        {
            Assert(() =>
            {
                if (items.Length == 0)
                {
                    throw new EmptyDirectoryException(directoryPath);
                }
            });
        }
    }
}