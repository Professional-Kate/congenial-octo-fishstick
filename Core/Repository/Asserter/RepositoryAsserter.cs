using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Core.Repository.Asserter
{
    /// <seealso cref="IRepositoryAsserter"/>
    public class RepositoryAsserter(IFoundAssertion foundAssertion, IObjectNullAssertion objectNullAssertion, IUniqueAssertion uniqueAssertion)
        : IRepositoryAsserter
    {
        public void AssertUnique<T>(T context, bool alreadyExists)
        {
            objectNullAssertion.AssertNotNull(context, nameof(context));
            uniqueAssertion.AssertUnique(context, alreadyExists);
        }

        public void AssertFound<T>(T context, bool notFound)
        {
            objectNullAssertion.AssertNotNull(context, nameof(context));
            foundAssertion.AssertFound(context, notFound);
        }

        public void AssertNotNull<T>(T value)
        {
            objectNullAssertion.AssertNotNull(value, nameof(value));
        }
    }
}