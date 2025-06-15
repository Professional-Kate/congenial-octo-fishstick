using System.Collections.Generic;
using IdelPog.Common.DTO;
using IdelPog.Frontend.MonoGame.Mediator;

namespace IdelPog.Frontend.MonoGame.Controllers
{
    public class RenderingController : IRenderingController
    {
        private readonly IRenderingMediator _mediator;

        public RenderingController(IRenderingMediator mediator)
        {
            _mediator = mediator;
        }

        public void RenderTextures(IReadOnlyList<RenderableDTO> dtos)
        {
            _mediator.RenderEntities(dtos);
        }
    }
}