namespace IdelPog.Validation.Assertions
{
    /// <seealso cref="AssertContains"/>
    public interface IAssertNonDuplicate
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="alreadyContains"></param>
        public void AssertContains(object context, Func<bool> alreadyContains);
    }
}