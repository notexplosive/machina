using Machina.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    public struct GameSettings
    {
        public float MusicVolume
        {
            private set; get;
        }
        public float SoundVolume
        {
            private set; get;
        }
        public bool Fullscreen
        {
            private set; get;
        }

        public UIState<bool> fullscreenState;
        public UIState<int> musicVolumeState;
        public UIState<int> soundVolumeState;
        private Point savedWindowSize;

        public void Apply()
        {
            this.Fullscreen = fullscreenState.GetState();
            this.SoundVolume = soundVolumeState.GetState();
            this.MusicVolume = musicVolumeState.GetState();

            var graphics = MachinaGame.Graphics;

            if (!graphics.IsFullScreen)
            {
                this.savedWindowSize = new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            }

            if (Fullscreen)
            {
                graphics.PreferredBackBufferWidth = MachinaGame.Current.GraphicsDevice.DisplayMode.Width;
                graphics.PreferredBackBufferHeight = MachinaGame.Current.GraphicsDevice.DisplayMode.Height;
                graphics.IsFullScreen = true;
            }
            else
            {
                graphics.PreferredBackBufferWidth = this.savedWindowSize.X;
                graphics.PreferredBackBufferHeight = this.savedWindowSize.Y;
                graphics.IsFullScreen = false;
            }
            graphics.ApplyChanges();
        }
    }
}
