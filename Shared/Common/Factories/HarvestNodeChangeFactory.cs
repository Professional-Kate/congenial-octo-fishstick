using IdelPog.Common.Commands;
using IdelPog.Common.Enums;

namespace IdelPog.Common.Factories
{
    public class HarvestNodeChangeFactory : IHarvestNodeChangeFactory
    {
        public HarvestNodeChange Create(ResourceID resourceID)
        {
            return new HarvestNodeChange
            {
                ResourceID = resourceID,
            };
        }
    }
}