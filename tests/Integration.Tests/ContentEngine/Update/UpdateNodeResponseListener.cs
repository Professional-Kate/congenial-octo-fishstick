using IdelPog.Common.Responses;
using IdelPog.Messaging.Listeners;

namespace Integration.Tests.ContentEngine.Update
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