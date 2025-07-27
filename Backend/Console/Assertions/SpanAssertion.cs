using Console.Runtime.Input.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace Console.Assertions
{
    public class SpanAssertion : BaseAssertion, ISpanAssertion
    {
        public SpanAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertNotEmpty<T>(ReadOnlySpan<T> span)
        {
            bool spanEmpty = span.IsEmpty;

            Assert<EmptySpanException>(() =>
            {
                if (spanEmpty)
                {
                    throw new EmptySpanException(typeof(T));
                }
            });
        }
    }
}