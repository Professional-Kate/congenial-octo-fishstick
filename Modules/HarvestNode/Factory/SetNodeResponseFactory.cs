using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Response;
using IdelPog.HarvestNode.Factory.Interface;

namespace IdelPog.HarvestNode.Factory
{
    public class SetNodeResponseFactory : ISetNodeResponseFactory
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