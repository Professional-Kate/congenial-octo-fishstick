using IdelPog.Common.DTO;
using IdelPog.Messaging.Listeners;

namespace Integration.Tests.ContentEngine
{
    public class HarvestNodeChangeDTOListener : ISingleListener<ResourceChangeDTO>
    {
        public Type ListenerType => typeof(ResourceChangeDTO);
        public bool WasCalled { get; private set; }
        public ResourceChangeDTO ResourceChangeDTO { get; private set; } 
        
        public void Handle(ResourceChangeDTO resource)
        {
            WasCalled = true;
            ResourceChangeDTO = resource;
        }

    }
}