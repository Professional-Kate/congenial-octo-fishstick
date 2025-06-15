using IdelPog.Messaging.Collection;

namespace IdelPog.Messaging.Assertions
{
    public interface IAssertBufferState
    {
        public void AssertState(BufferState expected, BufferState actual);
    }
}