using Frontend.MonoGame.Controllers;

namespace Frontend.MonoGame.Factory
{
    public class IdelPogFactory : IIdelPogFactory
    {
        private readonly IGameController _gameController;

        public IdelPogFactory()
        {
            _gameController = new GameController();
        }
        
        public void StartIdelPog()
        {
            _gameController.StartGame();
        }
    }
}