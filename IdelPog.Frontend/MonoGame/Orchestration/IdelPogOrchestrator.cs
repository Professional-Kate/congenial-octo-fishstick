using Frontend.MonoGame.Listeners;

namespace Frontend.MonoGame.Orchestration
{
    public class IdelPogOrchestrator : IIdelPogOrchestrator
    {
        private readonly GameRoot _gameRoot = new();
        private TextureIDListener _textureIDListener;

        public GameRoot BeginIdelPog()
        {
            _textureIDListener = new TextureIDListener();
            // TODO: work out a way to subscribe the listener. Should be automatic
            return _gameRoot;
        }
    }
}