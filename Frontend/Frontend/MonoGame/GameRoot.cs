using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IdelPog.Frontend.MonoGame
{
    public sealed class GameRoot : Game
    {
        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        public SpriteBatch SpriteBatch { get; private set; }

        public GameRoot()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            
            base.LoadContent();
        }

        protected override void Draw(GameTime gameTime)
        {
            _graphicsDeviceManager.GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    }
}