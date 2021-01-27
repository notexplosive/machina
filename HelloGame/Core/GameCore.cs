using HelloGame.Hello;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PrimitiveBuddy;

namespace HelloGame
{
    public class GameCore : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        Texture2D ballTexture;
        private Vector2 ballPosition;
        private Actor firstActor;
        Primitive prim;
        private KeyboardState previousKeys;

        public GameCore()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            firstActor = new Actor();
            firstActor.position = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);

            base.Initialize();
            prim = new Primitive(graphics.GraphicsDevice, spriteBatch)
            {
                NumCircleSegments = 16
            };
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load all textures and other content
            ballTexture = Content.Load<Texture2D>("ball");

            firstActor.texture = ballTexture;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var curKeys = Keyboard.GetState();

            firstActor.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

            if (previousKeys.IsKeyUp(Keys.A) && curKeys.IsKeyDown(Keys.A))
            {
                prim.NumCircleSegments++;
            }
            else if (previousKeys.IsKeyUp(Keys.S) && curKeys.IsKeyDown(Keys.S))
            {
                prim.NumCircleSegments--;
            }

            previousKeys = curKeys;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            firstActor.Draw(spriteBatch);

            prim.Circle(new Vector2(128f, 128f), 64f, Color.White);
            prim.Pie(new Vector2(256f, 256f), 64f, MathHelper.PiOver2, MathHelper.PiOver2, Color.White);

            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
