using Console.Commands.Domains.Arguments;

namespace Console.Runtime.Systems
{
    public interface IPermissionService
    {
        public void PermissionUpdate(PermissionUpdateArguments arguments);
    }
}