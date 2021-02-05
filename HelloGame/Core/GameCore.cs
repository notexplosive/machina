using Machina;
using Machina.Components;
using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace HelloGame
{
    public class GameCore : Game
    {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        static AssetLibrary assets;

        Scene gameScene;
        private Camera camera;

        public GameCore()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            assets = new AssetLibrary(this);
        }

        protected override void Initialize()
        {
            gameScene = new Scene();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            assets.LoadAllTextures();

            Actor ballActor = new Actor
            {
                position = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2)
            };

            Actor linkin = new Actor
            {
                position = new Vector2(200, 200)
            };

            this.camera = new Camera();

            gameScene.AddActor(linkin);
            new SpriteRenderer(linkin, new GridBasedSpriteSheet(assets.GetTexture("linkin"), new Point(16, 16)));

            var standAnim = new LinearFrameAnimation(0, 5);
            var walkAnim = new LinearFrameAnimation(6, 3);
            var quickSwingAnim = new LinearFrameAnimation(9, 3);
            var longSwingAnim = new LinearFrameAnimation(11, 5);
            var finalSwingAnim = new LinearFrameAnimation(16, 7, LoopType.HoldLastFrame);

            linkin.GetComponent<SpriteRenderer>().SetAnimation(walkAnim);

            new BoundingRect(linkin, 32, 32);
            new BoundingRectRenderer(linkin);

            linkin.GetComponent<SpriteRenderer>().SetupBoundingRect();

            var box = new Actor
            {
                position = new Vector2(50, 350)
            };

            new BoundingRect(box, new Point(32, 32), new Vector2(16, 16));
            new BoundingRectRenderer(box);

            gameScene.AddActor(box);

            gameScene.AddActor(ballActor);
            new TextureRenderer(ballActor, assets.GetTexture("ball"));
            new KeyboardMovement(ballActor);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // camera.AdjustZoom((float) (scrollDelta / 100) / 10);
            gameScene.Update((float) gameTime.ElapsedGameTime.TotalSeconds);

            base.Update(gameTime);
        }



        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.BackToFront, null, SamplerState.PointClamp, DepthStencilState.Default, null, null, camera.TranslationMatrix);
            gameScene.Draw(spriteBatch);
            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
