namespace IdelPog.Console.Exceptions
{
    public class EmptySpanException : Exception
    {
        private const string MESSAGE = "The span of type '{0}' was empty.";

        public readonly Type SpanType;

        public EmptySpanException(Type spanType) : base(string.Format(MESSAGE, spanType))
        {
            SpanType = spanType;
        }
    }
}