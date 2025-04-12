using IdelPog.Engine.Validation.Assertions;

namespace IdelPog.Engine.Validation.Pipelines
{
    /// <seealso cref="IRepositoryAsserter"/>
    public class RepositoryAsserter : IRepositoryAsserter
    {
        private readonly IAssertFound _assertFound;
        private readonly IAssertNotNull _assertNotNull;
        private readonly IAssertNonDuplicate _assertNonDuplicate;
        
        public RepositoryAsserter(IAssertFound assertFound, IAssertNotNull assertNotNull, IAssertNonDuplicate assertNonDuplicate)
        {
            _assertFound = assertFound;
            _assertNotNull = assertNotNull;
            _assertNonDuplicate = assertNonDuplicate;
        }

        public void AssertUnique(object context, Func<bool> alreadyExists)
        {
            _assertNotNull.AssertObjectNotNull(context);
            _assertNonDuplicate.AssertContains(context, alreadyExists);
        }

        public void AssertFound(object context, Func<bool> notFound)
        {
            _assertNotNull.AssertObjectNotNull(context);
            _assertFound.AssertItemIsFound(context, notFound);
        }
    }
}