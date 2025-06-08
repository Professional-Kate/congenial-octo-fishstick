using Frontend.Rendering.Structures;

namespace Frontend.Rendering.Service
{
    public interface IRendererService
    {
        public void RenderEntities(params RenderEntity[] entities);
    }
}