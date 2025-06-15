using Frontend.Content.Service;
using Frontend.MonoGame.Controllers;
using Frontend.MonoGame.Converter;
using Frontend.MonoGame.Listeners;
using Frontend.MonoGame.Mediator;
using Frontend.Rendering;
using Frontend.Rendering.Service;
using Frontend.Rendering.Structures.Enums;
using IdelPog.Infrastructure.Repository;
using IdelPog.Staging.Messaging;
using Microsoft.Xna.Framework.Graphics;

namespace Frontend.MonoGame
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