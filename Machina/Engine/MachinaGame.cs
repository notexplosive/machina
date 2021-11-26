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
        protected readonly GameSpecification specification;
        public MachinaRuntime Runtime { get; }

        private static MouseCursor pendingCursor;

        public MachinaGame(GameSpecification specification, GameCartridge gameCartridge, IPlatformContext platformContext)
        {
            this.specification = specification;
            this.gameCartridge = gameCartridge;
            Content.RootDirectory = "Content";

            var graphics = new GraphicsDeviceManager(this)
            {
                HardwareModeSwitch = false
            };

            Runtime = new MachinaRuntime(this, graphics, this.specification, platformContext);
            MachinaClient.Setup(new AssetLibrary(this), this.specification);
            platformContext.OnGameConstructed(this);
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
            Runtime.OnLoadContent(Window, this.gameCartridge, GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            Runtime.spriteBatch.Dispose();
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