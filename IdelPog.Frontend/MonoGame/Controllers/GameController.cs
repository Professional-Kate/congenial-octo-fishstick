using System;

namespace Frontend.MonoGame.Controllers
{
    public class GameController : IGameController, IDisposable
    {
        private readonly GameRoot _gameRoot = new();
        private bool _disposed;
        
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