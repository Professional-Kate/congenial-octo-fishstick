using Microsoft.Xna.Framework.Graphics;

namespace Frontend.Rendering
{
    public interface IRenderer
    {
        public void RenderTarget(RenderEntity entity, SpriteBatch spriteBatch);
    }
}