using IdelPog.Core.Messaging.Listener.Single;
using IdelPog.HarvestNode.Contracts.Error;

namespace IdelPog.Integration.Tests.HarvestNode.Unlock.Unlock
{
    public sealed class HarvestNodeUnlockErrorListener : ISingleListener<HarvestNodeUnlockError>
    {
        public Type ListenerType => typeof(HarvestNodeUnlockError);
        public bool WasCalled { get; private set; }
        public HarvestNodeUnlockError HarvestNodeRequirementsCreationError { get; private set; }

        public void Handle(HarvestNodeUnlockError message)
        {
            WasCalled = true;
            HarvestNodeRequirementsCreationError = message;
        }

    }
}