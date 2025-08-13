using IdelPog.Console.Command.Domain.Argument;

namespace IdelPog.Console.Runtime.System
{
    public interface IPermissionService
    {
        public void PermissionUpdate(PermissionUpdateArguments arguments);
    }
}