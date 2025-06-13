using Frontend.Content.Service;
using Frontend.Rendering.Structures.Enums;
using IdelPog.Infrastructure.Repository;
using SharpDX.Direct3D11;

namespace IdelPog.Frontend.Tests.Service
{
    [TestFixture]
    public class UITextureResolverTest
    {
        private IUITextureResolver _textureResolver { get; set; }
        private IAssetRepository<TextureID, Texture2D> _assetRepository { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _assetRepository = new AssetRepository<TextureID, Texture2D>();
            _textureResolver = new UITextureResolver(_assetRepository);
        }

        [Test]
        public void Positive_GetTexture_ReturnsTexture()
        {
            _assetRepository.Add(TextureID.AAA, new Texture2D(new , null));
        }
    }
}