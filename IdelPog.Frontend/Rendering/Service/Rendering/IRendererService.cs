using System.Collections.Generic;
using Frontend.Rendering.Structures;

namespace Frontend.Rendering.Service
{
    public interface IRendererService
    {
        public void RenderEntities(IReadOnlyList<RenderEntity> entities);
    }
}