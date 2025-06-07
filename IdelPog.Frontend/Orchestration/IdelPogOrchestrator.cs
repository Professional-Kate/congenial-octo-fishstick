using Frontend.Service;

namespace Frontend.Orchestration
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
            // TODO: update to render the opening UI
            _rendererService.RenderEntities();
                
            return _gameRoot;
        }
    }
}