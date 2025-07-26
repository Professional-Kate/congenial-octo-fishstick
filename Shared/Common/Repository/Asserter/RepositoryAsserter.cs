using IdelPog.Validation.Assertions;

namespace IdelPog.Common.Repository
{
    /// <seealso cref="IRepositoryAsserter"/>
    public class RepositoryAsserter(IFoundAssertion foundAssertion, IObjectNullAssertion objectNullAssertion, IUniqueAssertion uniqueAssertion)
        : IRepositoryAsserter
    {
        public void AssertUnique(object context, bool alreadyExists)
        {
            objectNullAssertion.AssertNotNull(context, nameof(context));
            uniqueAssertion.AssertUnique(context, alreadyExists);
        }

        public void AssertFound(object context, bool notFound)
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