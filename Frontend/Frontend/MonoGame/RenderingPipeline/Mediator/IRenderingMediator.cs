using System.Collections.Generic;
using IdelPog.Frontend.UI.Structures;

namespace IdelPog.Frontend.MonoGame.Mediator
{
    public interface IRenderingMediator
    {
        public void RenderEntities(IReadOnlyList<RenderableDTO> dtos);
    }
}