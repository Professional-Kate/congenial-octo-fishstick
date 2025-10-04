using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Listener.Buffer;

namespace IdelPog.Integration.Tests.ContentEngine
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