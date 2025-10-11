using IdelPog.Core.Progression;
using IdelPog.HarvestNode.Contracts;
using IdelPog.HarvestNode.Runtime.Factory.Interface;

namespace IdelPog.HarvestNode.Runtime.Factory
{
    public class HarvestNodeFactory : IHarvestNodeFactory
    {
        public Contracts.HarvestNode Create(ReadOnlyHarvestNode readOnlyHarvestNode)
        {
            ReadOnlyLevelable readOnlyLevelable = readOnlyHarvestNode.ReadOnlyLevelable;
            Levelable levelable = new(readOnlyLevelable.Level, readOnlyLevelable.Experience, readOnlyLevelable.NextLevelExperience, readOnlyLevelable.ExperiencePerAction);
            
            return new Contracts.HarvestNode
            {
                Levelable = levelable, 
                Information = readOnlyHarvestNode.Information, 
                ResourceID = readOnlyHarvestNode.ResourceID, 
                LocationID = readOnlyHarvestNode.LocationID
            };
        }
    }
}