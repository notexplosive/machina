namespace Machina.Engine.Assets
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Data;
    using Microsoft.Xna.Framework.Graphics;

    public class AssetLoader
    {
        private readonly List<UnloadedAsset> unloadedAssets = new List<UnloadedAsset>();
        private readonly List<UnloadedDrawLoopAssetCallback> drawAssets = new List<UnloadedDrawLoopAssetCallback>();
        private readonly AssetLibrary library;
        private int totalCount;

        public AssetLoader(AssetLibrary library)
        {
            this.library = library;
        }

        private void AddAsset(UnloadedAsset asset)
        {
            this.unloadedAssets.Add(asset);
            this.totalCount++;
        }

        private void AddDrawAsset(UnloadedDrawLoopAssetCallback asset)
        {
            this.drawAssets.Add(asset);
            this.totalCount++;
        }

        public void AddImagePath(string name)
        {
            AddAsset(new UnloadedTexture(name));
        }

        public void AddSpritefontPath(string name)
        {
            AddAsset(new UnloadedSpritefont(name));
        }

        public void AddSoundPath(string name)
        {
            AddAsset(new UnloadedSound(name));
        }

        public void UpdateLoadNextThing()
        {
            if (this.unloadedAssets.Count == 0)
            {
                return;
            }

            var assetToLoad = this.unloadedAssets[0];
            this.unloadedAssets.RemoveAt(0);
            assetToLoad.Load(library);
        }

        public void DrawLoadNextThing(SpriteBatch spriteBatch)
        {
            if (this.drawAssets.Count == 0)
            {
                return;
            }

            var assetToLoad = this.drawAssets[0];
            this.drawAssets.RemoveAt(0);

            assetToLoad.Load(library, spriteBatch);
        }

        public bool IsDoneUpdateLoading()
        {
            return this.unloadedAssets.Count == 0;
        }

        public float Progress()
        {
            return 1f - (float) (this.unloadedAssets.Count + this.drawAssets.Count) / this.totalCount;
        }

        public void AddMachinaAssetCallback(string assetPath, Func<IAsset> callback)
        {
            AddAsset(new UnloadedAssetCallback(assetPath, callback));
        }

        public void AddDrawAssetCallback(string assetPath, Func<SpriteBatch, IAsset> callback)
        {
            AddDrawAsset(new UnloadedDrawLoopAssetCallback(callback));
        }

        public void ForceLoadAsset(string assetPath)
        {
            int indexOfAsset = 0;
            bool found = false;
            foreach (var unloadedAsset in this.unloadedAssets)
            {
                if (assetPath == unloadedAsset.assetPath)
                {
                    found = true;
                    break;
                }
                indexOfAsset++;
            }

            if (found)
            {
                this.unloadedAssets[indexOfAsset].Load(this.library);
                this.unloadedAssets.RemoveAt(indexOfAsset);
                return;
            }
            else
            {
                throw new ArgumentException($"Could not find unloaded asset {assetPath}");
            }
        }

        public bool IsDoneDrawLoading()
        {
            return this.drawAssets.Count == 0;
        }

        private abstract class UnloadedAsset
        {
            public readonly string assetPath;

            protected UnloadedAsset(string path)
            {
                this.assetPath = path;
            }

            public abstract void Load(AssetLibrary library);
        }

        private class UnloadedDrawLoopAssetCallback
        {
            private readonly Func<SpriteBatch, IAsset> callback;

            public UnloadedDrawLoopAssetCallback(Func<SpriteBatch, IAsset> callback)
            {
                this.callback = callback;
            }

            public void Load(AssetLibrary library, SpriteBatch spriteBatch)
            {
                this.callback(spriteBatch);
            }
        }

        private class UnloadedAssetCallback : UnloadedAsset
        {
            private readonly Func<IAsset> callback;

            public UnloadedAssetCallback(string path, Func<IAsset> callback) : base(path)
            {
                this.callback = callback;
            }

            public override void Load(AssetLibrary library)
            {
                library.AddMachinaAsset(this.assetPath, this.callback());
            }
        }

        private class UnloadedTexture : UnloadedAsset
        {
            public UnloadedTexture(string path) : base(path)
            {
            }

            public override void Load(AssetLibrary library)
            {
                library.LoadTexture(this.assetPath);
            }
        }

        private class UnloadedSound : UnloadedAsset
        {
            public UnloadedSound(string path) : base(path)
            {
            }

            public override void Load(AssetLibrary library)
            {
                library.LoadSoundEffect(this.assetPath);
            }
        }

        private class UnloadedSpritefont : UnloadedAsset
        {
            public UnloadedSpritefont(string path) : base(path)
            {
            }

            public override void Load(AssetLibrary library)
            {
                library.LoadSpriteFont(this.assetPath);
            }
        }
    }
}