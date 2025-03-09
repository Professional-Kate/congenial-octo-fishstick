using System;

namespace IdelPog.Validation.Assertions.Interfaces
{
    /// <seealso cref="AssertUnique"/>
    public interface IAssertUniqueItem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="alreadyContains"></param>
        public void AssertUnique(object context, Func<bool> alreadyContains);
    }
}