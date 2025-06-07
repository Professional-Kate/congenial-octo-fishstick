using Frontend.Controllers;
using Frontend.Orchestration;
using Frontend.Service;

namespace Frontend
{
    public static class Program
    {
        public static void Main()
        {
            using GameController gameController = new(new IdelPogOrchestrator(new RendererService()));
            gameController.StartGame();
        }
    }
}