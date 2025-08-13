using IdelPog.Console.Types;

namespace IdelPog.Console.Runtime.System
{
    public interface IDomainPermissionChecker
    {
        public bool IsAllowed(Domain domain);
    }
}