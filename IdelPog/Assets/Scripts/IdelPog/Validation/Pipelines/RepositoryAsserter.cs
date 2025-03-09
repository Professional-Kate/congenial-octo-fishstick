using System;
using IdelPog.Validation.Assertions.Interfaces;
using IdelPog.Validation.Pipelines.Interfaces;

namespace IdelPog.Validation.Pipelines
{
    /// <seealso cref="IRepositoryAsserter"/>
    public class RepositoryAsserter : IRepositoryAsserter
    {
        private readonly IAssertFound _assertFound;
        private readonly IAssertNotNull _assertNotNull;
        private readonly IAssertUniqueItem _assertUniqueItem;
        
        public RepositoryAsserter(IAssertFound assertFound, IAssertNotNull assertNotNull, IAssertUniqueItem assertUniqueItem)
        {
            _assertFound = assertFound;
            _assertNotNull = assertNotNull;
            _assertUniqueItem = assertUniqueItem;
        }

        public void AssertUnique(object context, Func<bool> alreadyExists)
        {
            _assertNotNull.AssertObjectNotNull(context);
            _assertUniqueItem.AssertUnique(context, alreadyExists);
        }

        public void AssertFound(object context, Func<bool> notFound)
        {
            _assertNotNull.AssertObjectNotNull(context);
            _assertFound.AssertItemIsFound(context, notFound);
        }
    }
}