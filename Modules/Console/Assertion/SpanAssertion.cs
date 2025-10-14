using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Runtime.Input.Exceptions;

namespace IdelPog.Console.Assertion
{
    public sealed class SpanAssertion : ISpanAssertion
    {
        public void AssertNotEmpty<T>(ReadOnlySpan<T> span)
        {
            if (span.IsEmpty)
            {
                throw new EmptySpanException(typeof(T));
            }
        }
    }
}