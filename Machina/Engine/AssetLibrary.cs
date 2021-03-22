using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.IO;
using Machina.Data;
using Microsoft.Xna.Framework.Audio;

namespace Machina.Engine
{
    public class AssetLibrary
    {
        private readonly Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        private readonly Dictionary<string, SpriteFont> spriteFonts = new Dictionary<string, SpriteFont>();
        private readonly Dictionary<string, SoundEffect> soundEffects = new Dictionary<string, SoundEffect>();
        private readonly Dictionary<string, IAsset> assets = new Dictionary<string, IAsset>();
        private readonly ContentManager content;

        public AssetLibrary(Game game)
        {
            this.content = game.Content;
        }
        public List<string> GetFilesAtContentDirectory(string contentFolder, string extension = "*")
        {
            var result = new List<string>();

            DirectoryInfo dir = new DirectoryInfo(this.content.RootDirectory + "\\" + contentFolder);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException();
            }

            FileInfo[] files = dir.GetFiles("*." + extension);
            foreach (FileInfo file in files)
            {
                result.Add(Path.GetFileNameWithoutExtension(file.FullName));
            }

            return result;
        }

        public void LoadAllContent()
        {
            foreach (var imageName in GetFilesAtContentDirectory("images"))
            {
                LoadTexture("images/" + imageName);
            }

            foreach (var spriteFont in GetFilesAtContentDirectory("fonts", "xnb"))
            {
                LoadSpriteFont("fonts/" + spriteFont);
            }

            foreach (var spriteFont in GetFilesAtContentDirectory("sounds", "xnb"))
            {
                LoadSoundEffect("sounds/" + spriteFont);
            }
        }

        private void LoadTexture(string fullName)
        {
            var splitName = fullName.Split('/');
            var name = splitName[^1];

            var texture = this.content.Load<Texture2D>(fullName);
            textures.Add(name, texture);
            Console.WriteLine(string.Format("Loaded Texture: {0}", fullName));
        }

        private void LoadSpriteFont(string fullName)
        {
            var splitName = fullName.Split('/');
            var name = splitName[^1];

            var spriteFont = this.content.Load<SpriteFont>(fullName);
            spriteFonts.Add(name, spriteFont);
            Console.WriteLine(string.Format("Loaded SpriteFont: {0}", fullName));
        }

        private void LoadSoundEffect(string fullName)
        {
            var splitName = fullName.Split('/');
            var name = splitName[^1];

            var soundEffect = this.content.Load<SoundEffect>(fullName);
            soundEffects.Add(name, soundEffect);
            Console.WriteLine(string.Format("Loaded SoundEffect: {0}", fullName));
        }

        public void UnloadAssets()
        {
            foreach (var asset in this.assets.Values)
            {
                asset.OnCleanup();
            }
        }

        public Texture2D GetTexture(string name)
        {
            Debug.Assert(textures.ContainsKey(name), "No texture called `" + name + "` was found");
            return textures[name];
        }

        public SpriteFont GetSpriteFont(string name)
        {
            Debug.Assert(spriteFonts.ContainsKey(name), "No SpriteFont called `" + name + "` was found");
            return spriteFonts[name];
        }

        public SoundEffectInstance CreateSoundEffectInstance(string name)
        {
            return GetSoundEffect(name).CreateInstance();
        }

        public SoundEffect GetSoundEffect(string name)
        {
            Debug.Assert(soundEffects.ContainsKey(name), "No sound effect called `" + name + "` was found");
            return soundEffects[name];
        }

        public T GetMachinaAsset<T>(string name) where T : class, IAsset
        {
            Debug.Assert(assets.ContainsKey(name), "No MachinaAsset called `" + name + "` was found");
            return assets[name] as T;
        }

        public T AddMachinaAsset<T>(string name, T asset) where T : IAsset
        {
            Debug.Assert(!assets.ContainsKey(name), "Duplicate MachinaAsset: `" + name + "`");
            assets[name] = asset;
            return asset;
        }
    }
}
