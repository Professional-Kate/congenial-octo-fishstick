using IdelPog.Common.Responses;
using IdelPog.Common.Structures;

namespace IdelPog.Common.Factories
{
    public interface IHarvestNodeUpdateResponseFactory
    {
        public HarvestNodeUpdateResponse Create(HarvestNode harvestNode, bool hasLeveled);
    }
}