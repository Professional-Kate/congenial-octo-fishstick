using System.Collections.Generic;
using IdelPog.Common.DTO;

namespace IdelPog.Frontend.MonoGame.Mediator
{
    public interface IRenderingMediator
    {
        public void RenderEntities(IReadOnlyList<RenderableDTO> dtos);
    }
}