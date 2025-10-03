using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Listener.Buffer;

namespace IdelPog.Integration.Tests.ContentEngine.Unlock.Creation
{
    public sealed class RequirementsCreationResponseListener : IBufferListener<HarvestNodeRequirementsCreationResponse>
    {
        public Type ListenerType => typeof(HarvestNodeRequirementsCreationResponse);
        public bool WasCalled { get; private set; }
        public HarvestNodeRequirementsCreationResponse[] HarvestNodeRequirementsCreationResponses { get; private set; } = [];

        public void Handle(IReadOnlyList<HarvestNodeRequirementsCreationResponse> buffer)
        {
            WasCalled = true;
            HarvestNodeRequirementsCreationResponses = buffer.ToArray();
        }
    }
}