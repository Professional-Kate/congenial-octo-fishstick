using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Messaging.Listener.Single;

namespace IdelPog.Integration.Tests.HarvestNode.Unlock.Creation
{
    public sealed class RequirementsCreationErrorListener : ISingleListener<HarvestNodeRequirementsCreationError>
    {
        public Type ListenerType => typeof(HarvestNodeRequirementsCreationError);
        public bool WasCalled { get; private set; }
        public HarvestNodeRequirementsCreationError HarvestNodeRequirementsCreationError { get; private set; }

        public void Handle(HarvestNodeRequirementsCreationError message)
        {
            WasCalled = true;
            HarvestNodeRequirementsCreationError = message;
        }

    }
}