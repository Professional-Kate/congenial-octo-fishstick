using Console;
using Console.Runtime.Input;
using Console.Types;
using IdelPog.ECS.Exceptions;

namespace Integration.Tests.Console.Permission
{
    [TestFixture]
    public class PermissionDomainFlowTest : ManagedBuffer
    {
        private IInputHandler _inputHandler;
        
        [SetUp]
        public void Setup()
        {
            _inputHandler = ConsoleBootstrapper.Initialize(BufferManager);
        }

        private void VerifyPermissionDoesNotExist(Domain domain)
        {
            try
            {
               TestPermissionService.SendRemovePermissionCall(_inputHandler, domain);
            }
            catch (ComponentNotFoundException)
            {
            }
        }
        
        private void VerifyPermissionExists(Domain domain)
        {
            try
            {
                TestPermissionService.SendAddPermissionCall(_inputHandler, domain);
            }
            catch (ComponentAlreadyExistsException)
            {
            }
        }

        [TestCase(Domain.CURRENCY)]
        [TestCase(Domain.SKILL)]
        public void Positive_AddPermissionUpdate_AddsPermission_AllowsCommand(Domain domain)
        {
            VerifyPermissionDoesNotExist(domain);
            string[] arguments = ["permission", "add", domain.ToString()];
            Assert.DoesNotThrow(() => _inputHandler.Input(new ReadOnlySpan<string>(arguments)));
        }
        
        [TestCase(Domain.CURRENCY)]
        [TestCase(Domain.SKILL)]
        public void Positive_RemovePermissionUpdate_RemovesPermission_CommandThrows(Domain domain)
        {
            VerifyPermissionExists(domain);
            string[] arguments = ["permission", "remove", domain.ToString()];
            Assert.DoesNotThrow(() => _inputHandler.Input(new ReadOnlySpan<string>(arguments)));
        }
    }
}