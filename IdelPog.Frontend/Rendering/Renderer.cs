using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frontend.Rendering
{
    public class Renderer : IRenderer
    {
        public void RenderTarget(RenderEntity entity, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(entity.Texture, entity.Position, null, 
                entity.Tint, 0f, Vector2.Zero, entity.Size, SpriteEffects.None, 0f);
        }
    }
}