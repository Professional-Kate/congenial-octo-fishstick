namespace IdelPog.Validation.Exceptions
{
    public class MaxLevelException : Exception
    {
        private const string MESSAGE = "The passed entity {0} (from '{1}') is at max level! No more levels for now!!!";

        public readonly object ID;
        public readonly string SourceName;

        public MaxLevelException(object id, string sourceName) : base(string.Format(MESSAGE, id, sourceName))
        {
            ID = id;
            SourceName = sourceName;
        }
    }
}