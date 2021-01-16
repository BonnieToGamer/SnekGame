using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace newSnekGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Snek snek;
        private SpriteFont consolas;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 825;
            _graphics.ApplyChanges();
            snek = new Snek();
            snek.SnekINIT(GraphicsDevice);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            consolas = Content.Load<SpriteFont>("consolas");

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            snek.SnekUPDATE();


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            snek.SnekDRAW(_spriteBatch, _graphics, consolas);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
