using IdelPog.SimulationEngine.Currency.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.SimulationEngine.Currency.Assertions
{
    public class AssertCollectionNotEmpty(IHandler handler) : BaseAssertion<CollectionEmptyException>(handler), IAssertCollectionNotEmpty
    {
        public void Handle<T>(IReadOnlyList<T> collection)
        {
            Assert(() =>
            {
                if (collection.Count == 0)
                {
                    throw new CollectionEmptyException(typeof(T));
                }
            });
        } 
    }
}