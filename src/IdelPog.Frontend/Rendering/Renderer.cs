using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frontend.Rendering
{
    public class Renderer(SpriteBatch spriteBatch) : IRenderer
    {
        public void RenderTarget(RenderEntity entity)
        {
            spriteBatch.Draw(entity.Texture, entity.Position, null, 
                Color.Black, 0f, Vector2.Zero, entity.Size, SpriteEffects.None, 0f);
        }
    }
}