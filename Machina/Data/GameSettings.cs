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

        public void Apply(WindowInterface window)
        {
            var stateChanged = fullscreen.State != window.IsFullScreen;
            if (stateChanged)
            {
                window.SetFullscreen(fullscreen.State);
            }

            window.ApplyChanges();
        }

        public void ApplyAndSave(MachinaFileSystem fileSystem, WindowInterface window)
        {
            Apply(window);
            SaveSettings(fileSystem);
        }

        private void SaveSettings(MachinaFileSystem fileSystem)
        {
            fileSystem.WriteStringToAppData(JsonConvert.SerializeObject(this), "settings.json", true);
        }

        public void LoadSavedSettingsIfExist(MachinaFileSystem fileSystem, WindowInterface window)
        {
            try
            {
                var json = fileSystem.ReadTextAppDataThenLocal("settings.json").Result;
                var data = JsonConvert.DeserializeObject<GameSettings>(json);
                LoadFromData(data);
                Apply(window);
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