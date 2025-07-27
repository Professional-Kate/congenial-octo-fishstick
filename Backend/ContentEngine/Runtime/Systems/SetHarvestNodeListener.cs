using IdelPog.Common.Commands;
using IdelPog.Messaging.Listeners;

namespace ContentEngine.Runtime.Systems
{
    public class SetHarvestNodeListener : ISingleListener<SetHarvestNode>
    {
        private readonly IHarvestNodeAccessSystem _harvestNodeAccessSystem;

        public SetHarvestNodeListener(IHarvestNodeAccessSystem harvestNodeAccessSystem)
        {
            _harvestNodeAccessSystem = harvestNodeAccessSystem;
        }

        public Type ListenerType => typeof(SetHarvestNode);
        
        public void Handle(SetHarvestNode harvestNode)
        {
            _harvestNodeAccessSystem.UpdateHarvestNode(harvestNode);
        }
    }
}