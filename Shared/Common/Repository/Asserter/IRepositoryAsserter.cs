namespace IdelPog.Common.Repository
{
    public interface IRepositoryAsserter
    {
        public void AssertUnique(object context, bool alreadyExists);

        public void AssertFound(object context, bool notFound);

        public void AssertNotNull<T>(T value);
    }
}