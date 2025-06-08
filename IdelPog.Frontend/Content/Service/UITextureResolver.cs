using Frontend.Rendering.Structures.Enums;
using IdelPog.Infrastructure.Repository;
using SharpDX.Direct3D11;

namespace Frontend.Content.Service
{
    public class UITextureResolver : IUITextureResolver
    {
        private readonly IRepository<TextureID, Texture2D> _textureRepository;

        public UITextureResolver(IRepository<TextureID, Texture2D> textureRepository)
        {
            _textureRepository = textureRepository;
        }
        
        public Texture2D GetTexture(TextureID textureID)
        {
            throw new System.NotImplementedException();
        }
    }
}