using IdelPog.Core.Factory.Interface;
using IdelPog.HarvestNode.Contracts.Response;
using IdelPog.HarvestNode.Factory.Interface;

namespace IdelPog.HarvestNode.Factory
{
    public class NodeUpdateResponseFactory : INodeUpdateResponseFactory
    {
        private readonly ILevelProgressFactory  _levelProgressFactory;

        public NodeUpdateResponseFactory(ILevelProgressFactory levelProgressFactory)
        {
            _levelProgressFactory = levelProgressFactory;
        }

        public HarvestNodeUpdateResponse Create(Contracts.HarvestNode harvestNode, bool hasLeveled)
        {
            return new HarvestNodeUpdateResponse
            {
                HasLeveled = hasLeveled,
                ReadOnlyLevelable = _levelProgressFactory.CreateLevelProgress(harvestNode.Levelable),
                ResourceID = harvestNode.ResourceID,
                LocationID = harvestNode.LocationID
            };
        }
    }
}