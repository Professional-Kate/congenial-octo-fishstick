namespace IdelPog.Common.DTO.Factories
{
    public interface IResourceChangeErrorFactory
    {
        public ResourceChangeErrorDTO Create(ResourceChangeDTO resourceChangeDTO, Exception exception);
    }
}