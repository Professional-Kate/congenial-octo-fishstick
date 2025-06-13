using Frontend.Rendering.Structures.Enums;
using IdelPog.Infrastructure.Repository;
using SharpDX.Direct3D11;

namespace Frontend.Content.Service
{
    public class UITextureResolver : IUITextureResolver
    {
        private readonly IAssetRepository<TextureID, Texture2D> _textureRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textureRepository">A filled repository</param>
        /// <remarks>
        /// The <see cref="IAssetRepository{TID,T}"/> is expected to be filled on construct.
        /// This class offers no way to mutate the collection after construction
        /// </remarks>
        public UITextureResolver(IAssetRepository<TextureID, Texture2D> textureRepository)
        {
            _textureRepository = textureRepository;
        }
        
        public Texture2D GetTexture(TextureID textureID)
        {
            return _textureRepository.Get(textureID);
        }
    }
}