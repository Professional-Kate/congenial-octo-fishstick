using Console;
using Console.Runtime.Input;

namespace Integration.Tests.Console
{
    [TestFixture]
    public class SkillDomainFlowTest : ManagedBuffer
    {
        private IInputHandler _inputHandler;
        private SkillChangeListener _skillChangeListener;

        [SetUp]
        public void Setup()
        {
            _inputHandler = ConsoleBootstrapper.Initialize(BufferManager);
            
            _skillChangeListener = new SkillChangeListener();
            ManagedSubscribe(_skillChangeListener);
        }
    }
}