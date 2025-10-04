using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Listener.Buffer;

namespace IdelPog.Integration.Tests.SkillCommands.Update
{
    public sealed class SkillUpdateResponseListener : IBufferListener<SkillUpdateResponse>
    {
        public Type ListenerType => typeof(SkillUpdateResponse);
        public bool WasCalled { get; private set; }
        public SkillUpdateResponse[] SkillUpdateResponses { get; private set; } = null!;

        public void Handle(IReadOnlyList<SkillUpdateResponse> buffer)
        {
            WasCalled = true;
            SkillUpdateResponses = buffer.ToArray();
        }

    }
}