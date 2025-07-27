namespace IdelPog.Common.Repository
{
    public interface IRepositoryAsserter
    {
        public void AssertUnique<T>(T context, bool alreadyExists);

        public void AssertFound<T>(T context, bool notFound);

        public void AssertNotNull<T>(T value);
    }
}