using Microsoft.Xna.Framework;

namespace Frontend.Bootstraper
{
    public sealed class GameRoot : Game
    {
        private readonly GraphicsDeviceManager _graphicsDeviceManager;

        public GameRoot()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Draw(GameTime gameTime)
        {
            _graphicsDeviceManager.GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    }
}