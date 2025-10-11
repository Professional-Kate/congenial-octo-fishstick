using IdelPog.Core.Messaging.Listener.Single;
using IdelPog.Skill.Contracts.Error;

namespace IdelPog.Integration.Tests.SkillCommands.Update
{
    public sealed class SkillUpdateErrorListener : ISingleListener<SkillUpdateError>
    {
        public Type ListenerType => typeof(SkillUpdateError);
        public bool WasCalled { get; private set; }
        public SkillUpdateError SkillUpdateError { get; private set; }

        public void Handle(SkillUpdateError message)
        {
            WasCalled = true;
            SkillUpdateError = message;
        }

    }
}