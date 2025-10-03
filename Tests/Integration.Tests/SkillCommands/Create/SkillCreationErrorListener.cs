using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Messaging.Listener.Single;

namespace IdelPog.Integration.Tests.SkillCommands.Create
{
    public sealed class SkillCreationErrorListener : ISingleListener<SkillCreationError>
    {
        public Type ListenerType => typeof(SkillCreationError);
        public bool WasCalled { get; private set; }
        public SkillCreationError SkillCreationError { get; private set; }
        
        public void Handle(SkillCreationError message)
        {
            WasCalled = true;
            SkillCreationError = message;
        }

    }
}