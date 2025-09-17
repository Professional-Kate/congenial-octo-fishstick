using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Listener.Single;

namespace IdelPog.Integration.Tests.SkillCommands.Create
{
    public sealed class SkillCreationResponseListener : ISingleListener<SkillCreationResponse>
    {
        public Type ListenerType => typeof(SkillCreationResponse);
        public bool WasCalled { get; private set; }
        public SkillCreationResponse SkillCreationResponse { get; private set; }

        public void Handle(SkillCreationResponse message)
        {
            WasCalled = true;
            SkillCreationResponse = message;
        }

    }
}