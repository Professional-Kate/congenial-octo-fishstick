using IdelPog.Common.Enums;

namespace IdelPog.Common.DTO.Factories
{
    public class NodeChangeDTOFactory : INodeChangeDTOFactory
    {
        public ResourceChangeDTO Create(ResourceID resourceID)
        {
            return new  ResourceChangeDTO { ResourceID = resourceID };
        }
    }
}