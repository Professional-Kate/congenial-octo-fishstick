using IdelPog.Validation.Assertions.Interfaces;
using IdelPog.Validation.Interfaces;

namespace IdelPog.Validation.Pipelines.Interfaces
{
    public interface IRepositoryAsserter : IAssertNotNull, IAssertUniqueItem, IAssertFound
    {
    }
}