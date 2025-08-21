using IdelPog.Console.Resolver.Permission;

namespace IdelPog.Console.Runtime.System
{
    public interface IPermissionService
    {
        public void PermissionUpdate(PermissionUpdateArguments arguments);
    }
}