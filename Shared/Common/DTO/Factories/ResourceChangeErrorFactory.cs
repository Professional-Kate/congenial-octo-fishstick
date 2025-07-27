namespace IdelPog.Common.DTO.Factories
{
    public class ResourceChangeErrorFactory : IResourceChangeErrorFactory
    {
        private readonly ErrorDTOFactory _errorDTOFactory;

        public ResourceChangeErrorFactory(ErrorDTOFactory errorDTOFactory)
        {
            _errorDTOFactory = errorDTOFactory;
        }

        public ResourceChangeErrorDTO Create(ResourceChangeDTO resourceChangeDTO, Exception exception)
        {
            return new ResourceChangeErrorDTO
            {
                ResourceChangeDTO = resourceChangeDTO,
                ErrorDTO = _errorDTOFactory.Create(exception)
            };
        }
    }
}