using IdelPog.Common.Commands;
using IdelPog.Common.Responses;

namespace IdelPog.Common.Factories
{
    public class SetHarvestNodeResponseFactory : ISetHarvestNodeResponseFactory
    {
        public SetHarvestNodeResponse Create(SetHarvestNode setHarvestNode)
        {
            return new SetHarvestNodeResponse
            {
                SetHarvestNode = setHarvestNode
            };
        }
    }
}