using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Messaging.Listener.Single;

namespace IdelPog.Integration.Tests.HarvestNode.Loot.Location
{
    public sealed class LocationLootCreationErrorListener : ISingleListener<LocationLootCreationError>
    {
        public Type ListenerType => typeof(LocationLootCreationError);
        public bool WasCalled { get; private set; }
        public LocationLootCreationError LocationLootCreationError { get; private set; }
        
        public void Handle(LocationLootCreationError message)
        {
            WasCalled = true;
            LocationLootCreationError = message;
        }

    }
}