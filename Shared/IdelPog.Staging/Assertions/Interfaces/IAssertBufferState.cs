using IdelPog.Staging.Collection;

namespace IdelPog.Staging.Assertions
{
    public interface IAssertBufferState
    {
        public void AssertState(BufferState expected, BufferState actual);
    }
}