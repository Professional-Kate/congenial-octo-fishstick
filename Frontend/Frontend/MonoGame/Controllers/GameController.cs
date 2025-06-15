using System;

namespace IdelPog.Frontend.MonoGame.Controllers
{
    public class GameController : IGameController, IDisposable
    {
        private readonly GameRoot _gameRoot;
        private bool _disposed;

        public GameController(GameRoot gameRoot)
        {
            _gameRoot = gameRoot;
        }
        
        public void StartGame()
        {
            // TODO: call initial GameRoot methods here
        }
        
        protected virtual void Dispose(bool disposing)
        {
            // CA1816
            if (_disposed)
            {
                return;
            }

            if (disposing == false)
            {
                return;
            }
            
            _gameRoot.Dispose();
            _disposed = true;
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}