using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Listener.Buffer;

namespace IdelPog.Integration.Tests.HarvestNode
{
    public class NodeCreationResponseListener : IBufferListener<HarvestNodeCreationResponse>
    {
        public Type ListenerType => typeof(HarvestNodeCreationResponse);
        public bool WasCalled { get; private set; }
        public HarvestNodeCreationResponse[] HarvestNodeCreationResponses { get; private set; } = null!;

        public void Handle(IReadOnlyList<HarvestNodeCreationResponse> buffer)
        {
            WasCalled = true;
            HarvestNodeCreationResponses = buffer.ToArray();
        }
    }
}