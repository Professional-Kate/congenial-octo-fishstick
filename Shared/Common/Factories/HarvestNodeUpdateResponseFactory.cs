using IdelPog.Common.Responses;
using IdelPog.Common.Structures;

namespace IdelPog.Common.Factories
{
    public class HarvestNodeUpdateResponseFactory : IHarvestNodeUpdateResponseFactory
    {
        private readonly ILevelProgressFactory  _levelProgressFactory;

        public HarvestNodeUpdateResponseFactory(ILevelProgressFactory levelProgressFactory)
        {
            _levelProgressFactory = levelProgressFactory;
        }

        public HarvestNodeUpdateResponse Create(HarvestNode harvestNode, bool hasLeveled)
        {
            return new HarvestNodeUpdateResponse
            {
                HasLeveled = hasLeveled,
                LevelProgress = _levelProgressFactory.CreateLevelProgress(harvestNode.Levelable),
                ResourceID = harvestNode.ResourceID
            };
        }
    }
}