using Frontend.Rendering;

namespace Frontend.Service
{
    public interface IRendererService
    {
        public void RenderEntities(params RenderEntity[] entities);
    }
}