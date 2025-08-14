using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Response;

namespace IdelPog.HarvestNode.Factory.Interface
{
    public interface ISetNodeResponseFactory
    {
        public SetHarvestNodeResponse Create(SetHarvestNode setHarvestNode);
    }
}