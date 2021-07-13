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
    public interface IAssetLibrary
    {
        public T AddMachinaAsset<T>(string name, T asset) where T : IAsset;
        public T GetMachinaAsset<T>(string name) where T : class, IAsset;
        public SpriteFont GetSpriteFont(string name);
        public Texture2D GetTexture(string name);
        public void LoadAllContent();
        public void UnloadAssets();
        public SoundEffectInstance CreateSoundEffectInstance(string name);
        public SoundEffectInstance GetSoundEffectInstance(string name);
    }

    public class AssetLibrary : IAssetLibrary
    {
        private readonly Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();
        private readonly Dictionary<string, SpriteFont> spriteFonts = new Dictionary<string, SpriteFont>();
        private readonly Dictionary<string, SoundEffect> soundEffects = new Dictionary<string, SoundEffect>();
        private readonly Dictionary<string, SoundEffectInstance> soundEffectInstances = new Dictionary<string, SoundEffectInstance>();
        private readonly Dictionary<string, IAsset> assets = new Dictionary<string, IAsset>();
        private readonly ContentManager content;

        public AssetLibrary(MachinaGame game)
        {
            this.content = game.Content;
        }


        public void LoadAllContent()
        {
            foreach (var imageName in GamePlatform.GetFilesAtContentDirectory("images"))
            {
                LoadTexture("images/" + Path.GetFileNameWithoutExtension(imageName));
            }

            foreach (var spriteFont in GamePlatform.GetFilesAtContentDirectory("fonts", "xnb"))
            {
                LoadSpriteFont("fonts/" + Path.GetFileNameWithoutExtension(spriteFont));
            }

            foreach (var spriteFont in GamePlatform.GetFilesAtContentDirectory("sounds", "xnb"))
            {
                LoadSoundEffect("sounds/" + Path.GetFileNameWithoutExtension(spriteFont));
            }
        }

        private void LoadTexture(string fullName)
        {
            var name = Path.GetFileName(fullName);
            var texture = this.content.Load<Texture2D>(fullName);
            textures.Add(name, texture);
            Console.WriteLine(string.Format("Loaded Texture: {0} {1}", name, textures[name].GetHashCode()));
        }

        private void LoadSpriteFont(string fullName)
        {
            var name = Path.GetFileName(fullName);

            var spriteFont = this.content.Load<SpriteFont>(fullName);
            spriteFonts.Add(name, spriteFont);
            Console.WriteLine(string.Format("Loaded SpriteFont: {0}", name));
        }

        private void LoadSoundEffect(string fullName)
        {
            var name = Path.GetFileName(fullName);

            var soundEffect = this.content.Load<SoundEffect>(fullName);
            soundEffects.Add(name, soundEffect);
            soundEffectInstances.Add(name, soundEffect.CreateInstance());
            Console.WriteLine(string.Format("Loaded SoundEffect: {0}", name));
        }

        public void UnloadAssets()
        {
            foreach (var asset in this.assets.Values)
            {
                asset.OnCleanup();
            }

            foreach (var texture in this.textures.Values)
            {
                texture.Dispose();
            }

            foreach (var sfx in this.soundEffectInstances.Values)
            {
                sfx.Dispose();
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

        /// <summary>
        /// Gets a cached SoundEffectInstance created on launch
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SoundEffectInstance GetSoundEffectInstance(string name)
        {
            Debug.Assert(soundEffectInstances.ContainsKey(name), "No sound effect called `" + name + "` was found");
            return soundEffectInstances[name];
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
            Console.WriteLine("Added Machina Asset {0}", name);
            return asset;
        }
    }
}
