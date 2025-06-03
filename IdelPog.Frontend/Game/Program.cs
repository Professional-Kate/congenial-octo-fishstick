using Frontend.Controllers;
using Frontend.Orchestration;

namespace Frontend
{
    public static class Program
    {
        public static void Main()
        {
            using GameController gameController = new(new IdelPogOrchestrator());
            gameController.StartGame();
        }
    }
}