using IdelPog.Common.Enums;
using IdelPog.Common.Structures;

namespace ContentEngine.Runtime.Factory.Interfaces
{
    public interface IHarvestNodeFactory
    {
        public HarvestNode Create(ResourceID resourceID);
    }
}