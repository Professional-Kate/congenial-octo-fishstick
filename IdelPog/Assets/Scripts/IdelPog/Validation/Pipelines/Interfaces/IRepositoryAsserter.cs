using IdelPog.Validation.Assertions.Interfaces;

namespace IdelPog.Validation.Pipelines.Interfaces
{
    public interface IRepositoryAsserter : IAssertNotNull, IAssertUniqueItem, IAssertFound
    {
    }
}