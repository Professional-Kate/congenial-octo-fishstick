using Console.Runtime.Input;
using Console.Types;

namespace Integration.Tests.Console.Permission
{
    public abstract class TestPermissionService
    {
        public static void SendAddPermissionCall(IInputHandler inputHandler, Domain domain)
        {
            string[] arguments = ["permission", "add", domain.ToString()];
            inputHandler.Input(new ReadOnlySpan<string>(arguments));
        }

        public static void SendRemovePermissionCall(IInputHandler inputHandler, Domain domain)
        {
            string[] arguments = ["permission", "remove", domain.ToString()];
            inputHandler.Input(new ReadOnlySpan<string>(arguments));
        }
    }
}