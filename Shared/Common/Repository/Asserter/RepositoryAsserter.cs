using IdelPog.Validation.Assertions;

namespace IdelPog.Common.Repository
{
    /// <seealso cref="IRepositoryAsserter"/>
    public class RepositoryAsserter(IAssertFound assertFound, IAssertNotNull assertNotNull, IAssertNonDuplicate assertNonDuplicate)
        : IRepositoryAsserter
    {
        public void AssertUnique(object context, Func<bool> alreadyExists)
        {
            assertNotNull.AssertObjectNotNull(context);
            assertNonDuplicate.AssertContains(context, alreadyExists);
        }

        public void AssertFound(object context, Func<bool> notFound)
        {
            assertNotNull.AssertObjectNotNull(context);
            assertFound.AssertItemIsFound(context, notFound);
        }
    }
}