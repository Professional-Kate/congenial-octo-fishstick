using System.Collections.Generic;
using Frontend.UI.Structures;

namespace Frontend.MonoGame.Mediator
{
    public interface IRenderingMediator
    {
        public void RenderEntities(IReadOnlyList<RenderableDTO> dtos);
    }
}