using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Factory.Interface;
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
                LevelProgress = _levelProgressFactory.CreateLevelProgress(harvestNode.Levelable),
                ItemID = harvestNode.ItemID
            };
        }
    }
}