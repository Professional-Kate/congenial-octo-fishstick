using System;

namespace IdelPog.Validation.Interfaces
{
    public interface IAssertFound
    {
        public void AssertItemIsFound(object key, Func<bool> itemNotFound);
    }
}