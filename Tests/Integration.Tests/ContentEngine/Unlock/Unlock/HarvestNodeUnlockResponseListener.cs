using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Listener.Buffer;

namespace IdelPog.Integration.Tests.ContentEngine.Unlock.Unlock
{
    public sealed class HarvestNodeUnlockResponseListener : IBufferListener<HarvestNodeUnlockResponse>
    {
        public Type ListenerType => typeof(HarvestNodeUnlockResponse);
        public bool WasCalled { get; private set; }
        public HarvestNodeUnlockResponse[] HarvestNodeRequirementsCreationResponses { get; private set; } = [];

        public void Handle(IReadOnlyList<HarvestNodeUnlockResponse> buffer)
        {
            WasCalled = true;
            HarvestNodeRequirementsCreationResponses = buffer.ToArray();
        }
    }
}