using System.Collections.Generic;
using IdelPog.Frontend.MonoGame.Mediator;
using IdelPog.Frontend.UI.Structures;

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