using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Information.Contracts;
using IdelPog.Core.Progression;
using IdelPog.HarvestNode.Runtime.Factory.Interfaces;

namespace IdelPog.HarvestNode.Runtime.Factory
{
    public class HarvestNodeFactory : IHarvestNodeFactory
    {
        public Contracts.HarvestNode Create(ItemID itemID)
        {
            // TODO: need a way to fetch these
            Information information = new() { Description = "", Name = "" };
            Levelable levelable = new(0, 0, 0, 0);
            
            return new Contracts.HarvestNode { ItemID = itemID, Information = information, Levelable = levelable };
        }
    }
}