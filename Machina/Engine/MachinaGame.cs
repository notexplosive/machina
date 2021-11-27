using System;
using System.IO;
using Machina.Components;
using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Machina.Engine
{
    using Assets;
    using Machina.Engine.Cartridges;

    public enum DebugLevel
    {
        Off, // Completely disabled, can be enabled with hotkey
        Passive, // Show Console Output
        Active // Render DebugDraws
    }

    public class MachinaGame : Game
    {
        /// <summary>
        /// Cartridge provided by client code
        /// </summary>
        public readonly GameCartridge gameCartridge;
        private readonly IPlatformContext platformContext;
        protected readonly GameSpecification specification;
        public MachinaRuntime Runtime { get; private set; }

        private static MouseCursor pendingCursor;

        public MachinaGame(GameSpecification specification, GameCartridge gameCartridge, IPlatformContext platformContext, string devContentPath = "")
        {
            this.specification = specification;
            this.gameCartridge = gameCartridge;
            this.platformContext = platformContext;
            Content.RootDirectory = "Content";

            var graphics = new GraphicsDeviceManager(this)
            {
                HardwareModeSwitch = false
            };
            MachinaClient.Setup(new AssetLibrary(this), this.specification, graphics, devContentPath);

            this.platformContext.OnGameConstructed(this);
        }

        public static void SetCursor(MouseCursor cursor)
        {
            pendingCursor = cursor;
        }

        protected override void Initialize()
        {
            Window.Title = this.specification.gameTitle;
            IsMouseVisible = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // GraphicsDevice is not available until now
            var windowInterface = new OSWindow(this.specification.settings.startingWindowSize, Window, MachinaClient.Graphics, GraphicsDevice);
            Runtime = new MachinaRuntime(this, this.specification, platformContext, new Painter(GraphicsDevice));
            Runtime.LateSetup(this.gameCartridge, windowInterface);
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            Runtime.Painter.SpriteBatch.Dispose();
            MachinaClient.Assets.UnloadAssets();
        }

        protected override void Update(GameTime gameTime)
        {
            pendingCursor = MouseCursor.Arrow;
            var dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
            Runtime.Update(dt);

            Mouse.SetCursor(pendingCursor);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Runtime.Draw();
            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            foreach (var scene in Runtime.CurrentCartridge.SceneLayers.AllScenes())
            {
                scene.OnDeleteFinished();
            }
        }
    }
}