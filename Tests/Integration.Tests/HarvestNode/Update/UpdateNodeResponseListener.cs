using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.HarvestNode.Contracts.Response;

namespace IdelPog.Integration.Tests.HarvestNode
{
    internal class UpdateNodeResponseListener : IBufferListener<HarvestNodeUpdateResponse>
    {
        public Type ListenerType => typeof(HarvestNodeUpdateResponse);
        public bool WasCalled { get; private set; }
        public HarvestNodeUpdateResponse[] HarvestNodeUpdateResponses { get; private set; } = null!;

        public void Handle(IReadOnlyList<HarvestNodeUpdateResponse> buffer)
        {
            WasCalled = true;
            HarvestNodeUpdateResponses = buffer.ToArray();
        }
    }
}