using Frontend.Rendering.Service;

namespace Frontend.Game.Orchestration
{
    public class IdelPogOrchestrator : IIdelPogOrchestrator
    {
        private readonly GameRoot _gameRoot;
        private readonly IRendererService _rendererService;

        public IdelPogOrchestrator(IRendererService rendererService)
        {
            _gameRoot = new GameRoot();
            _rendererService = rendererService;
        }
        
        public GameRoot BeginIdelPog()
        {
            _rendererService.RenderEntities();
                
            return _gameRoot;
        }
    }
}