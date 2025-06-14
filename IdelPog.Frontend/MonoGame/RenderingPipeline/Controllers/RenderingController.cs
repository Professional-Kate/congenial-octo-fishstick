using System.Collections.Generic;
using Frontend.MonoGame.Mediator;
using Frontend.UI.Structures;

namespace Frontend.MonoGame.Controllers
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