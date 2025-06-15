using Frontend.MonoGame.Controllers;

namespace Frontend.MonoGame.Factory
{
    public class IdelPogFactory : IIdelPogFactory
    {
        private readonly IGameController _gameController;

        public IdelPogFactory(IGameController gameController)
        {
            _gameController = gameController;
        }
        
        public void StartIdelPog()
        {
            _gameController.StartGame();
        }
    }
}