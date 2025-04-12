namespace IdelPog.Engine.Validation.Pipelines.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="AssertUnique"/>
    /// <seealso cref="AssertFound"/>
    public interface IRepositoryAsserter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="alreadyExists"></param>
        public void AssertUnique(object context, Func<bool> alreadyExists);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="notFound"></param>
        public void AssertFound(object context, Func<bool> notFound);
    }
}