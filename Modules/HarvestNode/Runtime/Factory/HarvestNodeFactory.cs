using IdelPog.Core.Contracts;
using IdelPog.Core.Progression;
using IdelPog.HarvestNode.Runtime.Factory.Interfaces;

namespace IdelPog.HarvestNode.Runtime.Factory
{
    public class HarvestNodeFactory : IHarvestNodeFactory
    {
        public Contracts.HarvestNode Create(ReadOnlyHarvestNode readOnlyHarvestNode)
        {
            ReadOnlyLevelable readOnlyLevelable = readOnlyHarvestNode.ReadOnlyLevelable;
            Levelable levelable = new(readOnlyLevelable.Level, readOnlyLevelable.Experience, readOnlyLevelable.NextLevelExperience, readOnlyLevelable.ExperiencePerAction);
            
            return new Contracts.HarvestNode { ItemID = readOnlyHarvestNode.ItemID, Information = readOnlyHarvestNode.Information, Levelable = levelable, HarvestNodeID = readOnlyHarvestNode.HarvestNodeID };
        }
    }
}