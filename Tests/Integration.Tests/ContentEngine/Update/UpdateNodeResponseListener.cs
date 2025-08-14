using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Listener.Single;

namespace IdelPog.Integration.Tests.ContentEngine
{
    internal class UpdateNodeResponseListener : ISingleListener<HarvestNodeUpdateResponse>
    {
        public Type ListenerType => typeof(HarvestNodeUpdateResponse);
        public bool WasCalled { get; private set; }
        public HarvestNodeUpdateResponse HarvestNodeUpdateResponse { get; private set; }

        public void Handle(HarvestNodeUpdateResponse message)
        {
            WasCalled = true;
            HarvestNodeUpdateResponse = message;
        }
    }
}