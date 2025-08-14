using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Runtime.Input.Exceptions;
using IdelPog.Core.Validation;
using IdelPog.Core.Validation.Handler.Interface;

namespace IdelPog.Console.Assertion
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