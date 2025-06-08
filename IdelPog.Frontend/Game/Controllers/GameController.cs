using System;
using Frontend.Game.Orchestration;

namespace Frontend.Game.Controllers
{
    public class GameController(IIdelPogOrchestrator idelPogOrchestrator) : IGameController, IDisposable
    {
        private GameRoot _gameRoot;
        private bool _disposed;
        
        public void StartGame()
        {
            _gameRoot = idelPogOrchestrator.BeginIdelPog();
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
            _gameRoot = null;
            _disposed = true;
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}