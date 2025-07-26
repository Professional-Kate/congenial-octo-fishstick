using Console.Commands.Domains.Arguments;
using Console.Types;

namespace Console.Runtime.Systems
{
    public interface IPermissionService
    {
        public void PermissionUpdate(PermissionUpdateArguments arguments);
    }
}