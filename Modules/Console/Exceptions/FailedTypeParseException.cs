namespace IdelPog.Console.Exceptions
{
    public class FailedTypeParseException : Exception
    {
        private const string MESSAGE = "Couldn't parse \"{0}\" into type {1}.";
        public readonly string Argument;
        public readonly Type Type;

        public FailedTypeParseException(string argument, Type type) : base(string.Format(MESSAGE, argument, type))
        {
            Argument = argument;
            Type = type;
        }
    }
}