using System.Collections.Generic;
using IdelPog.Frontend.Rendering.Structures;

namespace IdelPog.Frontend.Rendering.Service
{
    public interface IRendererService
    {
        public void RenderEntities(IReadOnlyList<RenderEntity> entities);
    }
}