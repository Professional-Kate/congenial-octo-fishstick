namespace IdelPog.ContentHydrator.Exceptions
{
    public class EmptyDirectoryException : Exception
    {
        private const string MESSAGE = "The passed Directory path '{0}' is empty!!!";

        public readonly string Path;

        public EmptyDirectoryException(string path) : base(string.Format(MESSAGE, path))
        {
            Path = path;
        }
    }
}