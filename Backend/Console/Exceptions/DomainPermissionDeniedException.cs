using Console.Types;

namespace Console.Exceptions
{
    public class DomainPermissionDeniedException : Exception
    {
        private const string MESSAGE = "You don't have permission to access domain: {0}!!!!";
        
        public readonly CommandDomain PermissionDeniedDomain;

        public DomainPermissionDeniedException(CommandDomain commandDomain) : base(string.Format(MESSAGE, commandDomain))
        {
            PermissionDeniedDomain = commandDomain;
        }
    }
}