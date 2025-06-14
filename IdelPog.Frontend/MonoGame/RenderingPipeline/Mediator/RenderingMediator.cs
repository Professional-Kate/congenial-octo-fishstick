using System.Collections.Generic;
using Frontend.MonoGame.Converter;
using Frontend.Rendering.Service;
using Frontend.Rendering.Structures;
using Frontend.UI.Structures;

namespace Frontend.MonoGame.Mediator
{
    public class RenderingMediator : IRenderingMediator
    {
        private readonly IRenderableDTOConverter _renderableDTOConverter;
        private readonly IRendererService _rendererService;

        public RenderingMediator(IRenderableDTOConverter renderableDTOConverter, IRendererService rendererService)
        {
            _renderableDTOConverter = renderableDTOConverter;
            _rendererService = rendererService;
        }

        public void RenderEntities(IReadOnlyList<RenderableDTO> dtos)
        { 
            IReadOnlyList<RenderEntity> entities = _renderableDTOConverter.ConvertCollection(dtos);
            
            _rendererService.RenderEntities(entities);
        }
    }
}