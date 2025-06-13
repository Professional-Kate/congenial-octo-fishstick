using Frontend.MonoGame.Controllers;
using Frontend.MonoGame.Orchestration;
using Frontend.Rendering;
using Frontend.Rendering.Service;

namespace Frontend.MonoGame.Factory
{
    public class IdelPogFactory : IIdelPogFactory
    {
        private readonly IGameController _gameController;
        private readonly GameRoot _gameRoot;

        public IdelPogFactory()
        {
            _gameRoot = new GameRoot();
            _gameController = new GameController(new IdelPogOrchestrator(new RendererService(new Renderer(), _gameRoot.SpriteBatch)));
        }
        
        public void StartIdelPog()
        {
            _gameController.StartGame();
        }
    }
}