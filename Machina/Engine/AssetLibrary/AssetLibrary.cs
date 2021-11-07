namespace Machina.Engine.AssetLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using Data;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Media;

    public class AssetLibrary : IAssetLibrary
    {
        private readonly Dictionary<string, IAsset> assets = new Dictionary<string, IAsset>();
        private readonly ContentManager content;

        private readonly Dictionary<string, SoundEffectInstance> soundEffectInstances =
            new Dictionary<string, SoundEffectInstance>();

        private readonly Dictionary<string, SoundEffect> soundEffects = new Dictionary<string, SoundEffect>();
        private readonly Dictionary<string, SpriteFont> spriteFonts = new Dictionary<string, SpriteFont>();
        private readonly Dictionary<string, Texture2D> textures = new Dictionary<string, Texture2D>();

        public AssetLibrary(MachinaGame game)
        {
            this.content = game.Content;
        }

        public static AssetLoadTree GetStaticAssetLoadTree()
        {
            var loadTree = new AssetLoadTree();
            foreach (var imageName in GamePlatform.GetFilesAtContentDirectory("images"))
            {
                loadTree.AddImagePath("images/" + Path.GetFileNameWithoutExtension(imageName));
            }
            
            foreach (var spriteFont in GamePlatform.GetFilesAtContentDirectory("fonts", "xnb"))
            {
                loadTree.AddSpritefontPath("fonts/" + Path.GetFileNameWithoutExtension(spriteFont));
            }

            foreach (var soundEffect in GamePlatform.GetFilesAtContentDirectory("sounds", "xnb"))
            {
                loadTree.AddSoundPath("sounds/" + Path.GetFileNameWithoutExtension(soundEffect));
            }

            return loadTree;
        }

        public void LoadAllContent()
        {
            var loadTree = GetStaticAssetLoadTree();
            loadTree.LoadEverythingAtOnce(this);
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
            Debug.Assert(this.textures.ContainsKey(name), "No texture called `" + name + "` was found");
            return this.textures[name];
        }

        public SpriteFont GetSpriteFont(string name)
        {
            Debug.Assert(this.spriteFonts.ContainsKey(name), "No SpriteFont called `" + name + "` was found");
            return this.spriteFonts[name];
        }

        public SoundEffectInstance CreateSoundEffectInstance(string name)
        {
            return GetSoundEffect(name).CreateInstance();
        }

        public SoundEffect GetSoundEffect(string name)
        {
            Debug.Assert(this.soundEffects.ContainsKey(name), "No sound effect called `" + name + "` was found");
            return this.soundEffects[name];
        }

        /// <summary>
        ///     Gets a cached SoundEffectInstance created on launch
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public SoundEffectInstance GetSoundEffectInstance(string name)
        {
            Debug.Assert(this.soundEffectInstances.ContainsKey(name),
                "No sound effect called `" + name + "` was found");
            return this.soundEffectInstances[name];
        }

        public T GetMachinaAsset<T>(string name) where T : class, IAsset
        {
            Debug.Assert(this.assets.ContainsKey(name), "No MachinaAsset called `" + name + "` was found");
            return this.assets[name] as T;
        }

        public T AddMachinaAsset<T>(string name, T asset) where T : IAsset
        {
            Debug.Assert(!this.assets.ContainsKey(name), "Duplicate MachinaAsset: `" + name + "`");
            this.assets[name] = asset;
            Console.WriteLine("Added Machina Asset {0}", name);
            return asset;
        }

        public void LoadTexture(string fullName)
        {
            var name = Path.GetFileName(fullName);
            var texture = this.content.Load<Texture2D>(fullName);
            this.textures.Add(name, texture);
            Console.WriteLine("Loaded Texture: {0} {1}", name, this.textures[name].GetHashCode());
        }

        public void LoadSpriteFont(string fullName)
        {
            var name = Path.GetFileName(fullName);

            var spriteFont = this.content.Load<SpriteFont>(fullName);
            this.spriteFonts.Add(name, spriteFont);
            Console.WriteLine("Loaded SpriteFont: {0}", name);
        }

        public void LoadSoundEffect(string fullName)
        {
            var name = Path.GetFileName(fullName);

            var soundEffect = this.content.Load<SoundEffect>(fullName);
            this.soundEffects.Add(name, soundEffect);
            this.soundEffectInstances.Add(name, soundEffect.CreateInstance());
            Console.WriteLine("Loaded SoundEffect: {0}", name);
        }
    }
}
