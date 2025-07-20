using Console.Types;

namespace Console.Runtime.Systems
{
    public interface IDomainPermissionChecker
    {
        public bool IsAllowed(Domain domain);
    }
}