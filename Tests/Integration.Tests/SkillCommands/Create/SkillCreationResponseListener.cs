using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Listener.Buffer;

namespace IdelPog.Integration.Tests.SkillCommands.Create
{
    public sealed class SkillCreationResponseListener : IBufferListener<SkillCreationResponse>
    {
        public Type ListenerType => typeof(SkillCreationResponse);
        public bool WasCalled { get; private set; }
        public IReadOnlyList<SkillCreationResponse> SkillCreationResponses { get; private set; } = null!;

        public void Handle(IReadOnlyList<SkillCreationResponse> buffer)
        {
            WasCalled = true;
            SkillCreationResponses = buffer;
        }
    }
}