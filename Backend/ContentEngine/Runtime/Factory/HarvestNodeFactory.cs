using ContentEngine.Runtime.Factory.Interfaces;
using IdelPog.Common.Enums;
using IdelPog.Common.Structures;

namespace ContentEngine.Runtime.Factory
{
    public class HarvestNodeFactory : IHarvestNodeFactory
    {
        public HarvestNode Create(ResourceID resourceID)
        {
            // TODO: need a way to fetch these
            Information information = new() { Description = "", Name = "" };
            Levelable levelable = new(0, 0, 0, 0);
            
            return new HarvestNode { ResourceID = resourceID, Information = information, Levelable = levelable };
        }
    }
}