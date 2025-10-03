using IdelPog.Console;
using IdelPog.Console.Runtime.Input;
using IdelPog.Console.Types;
using IdelPog.ECS.Exceptions;

namespace IdelPog.Integration.Tests.Console.Permission
{
    [TestFixture]
    public class PermissionDomainFlowTest : ManagedTestBuffer
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

        [Test]
        public void Negative_AddPermissionUpdate_AlreadyExistingPermission_Throws()
        {
            Assert.Throws<ComponentAlreadyExistsException>(() => TestPermissionService.SendAddPermissionCall(_inputHandler, Domain.PERMISSION));
        }

        [Test]
        public void Negative_RemovePermissionUpdate_PermissionNotFound_Throws()
        {
            VerifyPermissionDoesNotExist(Domain.CURRENCY);
            Assert.Throws<ComponentNotFoundException>(() => TestPermissionService.SendRemovePermissionCall(_inputHandler, Domain.CURRENCY));
        }
    }
}