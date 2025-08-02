using IdelPog.Common.Responses;
using IdelPog.Messaging.Listeners;

namespace Integration.Tests.ContentEngine
{
    public class HarvestNodeChangeDTOListener : ISingleListener<SetHarvestNodeResponse>
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