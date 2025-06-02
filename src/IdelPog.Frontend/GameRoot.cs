using Microsoft.Xna.Framework;

namespace Frontend
{
    public sealed class GameRoot : Game
    {
        public GameRoot()
        {
            new GraphicsDeviceManager(this);
            Content.RootDirectory = "./Content";
            IsMouseVisible = true;
        }
    }
}