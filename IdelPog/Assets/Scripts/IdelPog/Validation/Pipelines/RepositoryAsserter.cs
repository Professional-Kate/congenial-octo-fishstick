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
        
        public void AssertObjectNotNull(object objectToAssert)
        {
            _assertNotNull.AssertObjectNotNull(objectToAssert);
        }

        public void AssertUnique(object context, Func<bool> alreadyContains)
        {
            _assertUniqueItem.AssertUnique(context, alreadyContains);
        }

        public void AssertItemIsFound(object key, Func<bool> itemNotFound)
        {
            _assertFound.AssertItemIsFound(key, itemNotFound);
        }
    }
}