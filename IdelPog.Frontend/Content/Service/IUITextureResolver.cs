using Frontend.Rendering.Structures.Enums;
using SharpDX.Direct3D11;

namespace Frontend.Content.Service
{
    public interface IUITextureResolver
    {
        public Texture2D GetTexture(TextureID textureID);
    }
}