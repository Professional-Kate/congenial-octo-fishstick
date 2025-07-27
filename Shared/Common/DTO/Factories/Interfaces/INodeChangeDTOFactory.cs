using IdelPog.Common.Enums;

namespace IdelPog.Common.DTO.Factories
{
    public interface INodeChangeDTOFactory
    {
        public ResourceChangeDTO Create(ResourceID resourceID);
    }
}