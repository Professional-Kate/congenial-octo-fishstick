using Frontend.Rendering;
using Microsoft.Xna.Framework.Graphics;

namespace Frontend.Service
{
    public class RendererService : IRendererService
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly IRenderer _renderer;

        public RendererService(IRenderer renderer, SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
            _renderer = renderer;
        }
        
        public void RenderEntities(params RenderEntity[] entities)
        {
            _spriteBatch.Begin();
            
            foreach (RenderEntity renderEntity in entities)
            {
                _renderer.RenderTarget(renderEntity, _spriteBatch);
            }
            
            _spriteBatch.End();
        }
    }
}