using Frontend.MonoGame.Controllers;
using Frontend.MonoGame.Orchestration;

namespace Frontend.MonoGame.Factory
{
    public class IdelPogFactory : IIdelPogFactory
    {
        private readonly IGameController _gameController;

        public IdelPogFactory()
        {
            _gameController = new GameController(new IdelPogOrchestrator());
        }
        
        public void StartIdelPog()
        {
            _gameController.StartGame();
        }
    }
}