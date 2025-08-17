using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Messaging.Listener.Single;

namespace IdelPog.Integration.Tests.Inventory
{
    public class InventoryUpdateErrorListener : ISingleListener<InventoryUpdateError>
    {
        public Type ListenerType => typeof(InventoryUpdateError);
        public bool WasCalled { get; private set; }
        public InventoryUpdateError InventoryUpdateError { get; private set; }

        public void Handle(InventoryUpdateError message)
        {
            System.Console.WriteLine(message.BaseError.Exception);
            WasCalled = true;
            InventoryUpdateError = message;
        }
    }
}