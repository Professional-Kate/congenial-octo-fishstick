using System.Collections.Generic;
using IdelPog.Common.DTO;
using IdelPog.Frontend.MonoGame.Converter;
using IdelPog.Frontend.Rendering.Service;
using IdelPog.Frontend.Rendering.Structures;

namespace IdelPog.Frontend.MonoGame.Mediator
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