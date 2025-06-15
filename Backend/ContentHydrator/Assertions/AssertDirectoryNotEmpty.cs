using IdelPog.ContentHydrator.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.ContentHydrator.Assertions
{
    /// <inheritdoc cref="IAssertDirectoryNotEmpty"/>
    public class AssertDirectoryNotEmpty(IHandler handler) : BaseAssertion<EmptyDirectoryException>(handler), IAssertDirectoryNotEmpty
    {
        public void AssertNotEmpty(string[] items, string directoryPathContext)
        {
            Assert(() =>
            {
                if (items.Length == 0)
                {
                    throw new EmptyDirectoryException(directoryPathContext);
                }
            });
        }
    }
}