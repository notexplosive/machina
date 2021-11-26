using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;

namespace Machina.Data
{
    [Serializable]
    public class GameSettings
    {
        public UIStateBool fullscreen = new UIStateBool(false);
        public UIStateInt masterVolume = new UIStateInt(5);
        public UIStateInt musicVolume = new UIStateInt(5);
        public UIStateInt sfxVolume = new UIStateInt(5);
        public readonly Point startingWindowSize;

        public GameSettings(Point startingWindowSize)
        {
            this.startingWindowSize = startingWindowSize;
        }

        public float MasterVolume => Math.Clamp(masterVolume.State / 10f, 0, 1);
        public float MusicVolumeAsFloat => Math.Clamp(musicVolume.State / 10f, 0, 1) * MasterVolume;
        public float SFXVolumeAsFloat => Math.Clamp(sfxVolume.State / 10f, 0, 1) * MasterVolume;

        public void Apply(MachinaRuntime Runtime)
        {
            var graphics = MachinaClient.Graphics;
            var device = Runtime.GraphicsDevice;
            var stateChanged = fullscreen.State != graphics.IsFullScreen;
            if (stateChanged)
            {
                if (fullscreen.State)
                {
                    graphics.PreferredBackBufferWidth = device.DisplayMode.Width;
                    graphics.PreferredBackBufferHeight = device.DisplayMode.Height;
                    graphics.IsFullScreen = true;
                }
                else
                {
                    graphics.PreferredBackBufferWidth = this.startingWindowSize.X;
                    graphics.PreferredBackBufferHeight = this.startingWindowSize.Y;
                    graphics.IsFullScreen = false;
                }
            }

            graphics.ApplyChanges();
        }

        public void ApplyAndSave(MachinaFileSystem fileSystem, MachinaRuntime runtime)
        {
            Apply(runtime);
            SaveSettings(fileSystem);
        }

        private void SaveSettings(MachinaFileSystem fileSystem)
        {
            fileSystem.WriteStringToAppData(JsonConvert.SerializeObject(this), "settings.json", true);
        }

        public void LoadSavedSettingsIfExist(MachinaFileSystem fileSystem, MachinaRuntime runtime)
        {
            try
            {
                var json = fileSystem.ReadTextAppDataThenLocal("settings.json").Result;
                var data = JsonConvert.DeserializeObject<GameSettings>(json);
                LoadFromData(data);
                Apply(runtime);
            }
            catch (Exception e)
            {
                MachinaClient.Print("Failed to load settings", e.Message);
            }
        }

        private void LoadFromData(GameSettings data)
        {
            musicVolume.State = data.musicVolume.State;
            sfxVolume.State = data.sfxVolume.State;
            fullscreen.State = data.fullscreen.State;
            masterVolume.State = data.masterVolume.State;
        }
    }
}