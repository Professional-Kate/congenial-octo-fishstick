using Console.Runtime.Input.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace Console.Runtime.Input.Assertions
{
    public class AssertSpanNotEmpty(IHandler handler) : BaseAssertion<EmptySpanException>(handler), IAssertSpanNotEmpty
    {
        public void Handle<T>(ReadOnlySpan<T> span)
        {
            bool spanEmpty = span.IsEmpty;
            
            Assert(() =>
            {
                if (spanEmpty)
                {
                    throw new EmptySpanException(typeof(T));
                }
            });
        }
    }
}