using Console.Types;

namespace Console.Exceptions
{
    public class DomainPermissionDeniedException : Exception
    {
        private const string MESSAGE = "You don't have permission to access domain: {0}!!!!";
        
        public readonly Domain PermissionDeniedDomain;

        public DomainPermissionDeniedException(Domain domain) : base(string.Format(MESSAGE, domain))
        {
            PermissionDeniedDomain = domain;
        }
    }
}