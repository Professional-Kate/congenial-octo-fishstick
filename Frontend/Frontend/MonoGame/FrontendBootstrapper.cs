using IdelPog.Common.Repository;
using IdelPog.Common.Structures;
using IdelPog.Frontend.Content.Service;
using IdelPog.Frontend.MonoGame.Controllers;
using IdelPog.Frontend.MonoGame.Converter;
using IdelPog.Frontend.MonoGame.Listeners;
using IdelPog.Frontend.MonoGame.Mediator;
using IdelPog.Frontend.Rendering;
using IdelPog.Frontend.Rendering.Service;
using IdelPog.Messaging.Listeners;
using Microsoft.Xna.Framework.Graphics;

namespace IdelPog.Frontend.MonoGame
{
    public class FrontendBootstrapper
    {
        public void Initialize(IBufferMessenger bufferMessenger)
        {
            GameRoot gameRoot = new();
            IGameController gameController = new GameController(gameRoot);
            
            // TODO: this needs to be filled before passing into the TextureResolver.
            IAssetRepository<TextureID, Texture2D> repository = new AssetRepository<TextureID, Texture2D>();
            IUITextureResolver textureResolver = new UITextureResolver(repository);
            IRenderableDTOConverter renderableDTOConverter = new RenderableDTOConverter(textureResolver);
            
            IRenderer renderer = new Renderer();
            IRendererService rendererService = new RendererService(renderer, gameRoot.SpriteBatch);
            
            IRenderingMediator renderingMediator = new RenderingMediator(renderableDTOConverter, rendererService);
            IRenderingController renderingController = new RenderingController(renderingMediator);
            RenderableDTOListener renderableDTOListener = new(renderingController);
            
            bufferMessenger.Subscribe(renderableDTOListener);
            gameController.StartGame();
        }
    }
}