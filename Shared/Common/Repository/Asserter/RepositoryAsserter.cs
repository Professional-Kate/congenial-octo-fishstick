using IdelPog.Validation.Assertions;

namespace IdelPog.Common.Repository
{
    /// <seealso cref="IRepositoryAsserter"/>
    public class RepositoryAsserter(IFoundAssertion foundAssertion, IObjectNullAssertion objectNullAssertion, IUniqueAssertion uniqueAssertion)
        : IRepositoryAsserter
    {
        public void AssertUnique(object context, Func<bool> alreadyExists)
        {
            objectNullAssertion.AssertNotNull(context, nameof(context));
            uniqueAssertion.AssertUnique<object>(alreadyExists);
        }

        public void AssertFound(object context, Func<bool> notFound)
        {
            objectNullAssertion.AssertNotNull(context, nameof(context));
            foundAssertion.AssertFound(context, notFound);
        }
    }
}