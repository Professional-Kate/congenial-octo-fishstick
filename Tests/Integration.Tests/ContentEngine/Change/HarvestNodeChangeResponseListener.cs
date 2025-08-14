using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Listener.Single;

namespace IdelPog.Integration.Tests.ContentEngine
{
    public class HarvestNodeChangeResponseListener : ISingleListener<SetHarvestNodeResponse>
    {
        public Type ListenerType => typeof(SetHarvestNodeResponse);
        public bool WasCalled { get; private set; }
        public SetHarvestNodeResponse SetHarvestNodeResponse { get; private set; } 
        
        public void Handle(SetHarvestNodeResponse message)
        {
            WasCalled = true;
            SetHarvestNodeResponse = message;
        }

    }
}