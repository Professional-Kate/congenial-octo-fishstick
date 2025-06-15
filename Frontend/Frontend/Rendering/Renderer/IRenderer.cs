using IdelPog.Frontend.Rendering.Structures;
using Microsoft.Xna.Framework.Graphics;

namespace IdelPog.Frontend.Rendering
{
    public interface IRenderer
    {
        public void RenderTarget(RenderEntity entity, SpriteBatch spriteBatch);
    }
}