using IdelPog.Common.Commands;
using IdelPog.Common.Enums;

namespace IdelPog.Common.Factories
{
    public interface IHarvestNodeChangeFactory
    {
        public HarvestNodeChange Create(ResourceID resourceID);
    }
}