namespace Console.Commands.Resolver.Exceptions
{
    public class FailedEnumParseException : Exception
    {
        private const string MESSAGE = "Couldn't parse \"{0}\". Expected one of the defined values in enum: {1}.";
        public readonly string Argument;
        public readonly string EnumName;

        public FailedEnumParseException(string argument, string enumName) : base(string.Format(MESSAGE, argument, enumName))
        {
            Argument = argument;
            EnumName = enumName;
        }
    }
}