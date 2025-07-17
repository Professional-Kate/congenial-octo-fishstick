using Console;
using Console.Runtime.Input;

namespace Integration.Tests.Console
{
    [TestFixture]
    public class ConsoleFlowTest
    {
        private IInputHandler _inputHandler;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _inputHandler = ConsoleBootstrapper.Initialize();
        }
    }
}