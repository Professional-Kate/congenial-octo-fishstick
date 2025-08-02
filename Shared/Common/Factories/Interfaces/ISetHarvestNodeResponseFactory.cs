using IdelPog.Common.Commands;
using IdelPog.Common.Responses;

namespace IdelPog.Common.Factories
{
    public interface ISetHarvestNodeResponseFactory
    {
        public SetHarvestNodeResponse Create(SetHarvestNode setHarvestNode);
    }
}