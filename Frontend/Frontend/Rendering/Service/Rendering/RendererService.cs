using System.Collections.Generic;
using IdelPog.Frontend.Rendering.Structures;
using Microsoft.Xna.Framework.Graphics;

namespace IdelPog.Frontend.Rendering.Service
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
        
        public void RenderEntities(IReadOnlyList<RenderEntity> entities)
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