namespace Machina.Engine.Assets
{
    using System;
    using System.Collections.Generic;
    using Data;
    using Microsoft.Xna.Framework.Graphics;

    public class AssetLoadTree
    {
        private abstract class UnloadedAsset
        {
            protected readonly string path;

            protected UnloadedAsset(string path)
            {
                this.path = path;
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
                library.AddMachinaAsset(this.path, this.callback());
            }
        }

        private class UnloadedTexture : UnloadedAsset
        {
            public UnloadedTexture(string path) : base(path)
            {
            }

            public override void Load(AssetLibrary library)
            {
                library.LoadTexture(this.path);
            }
        }

        private class UnloadedSound : UnloadedAsset
        {
            public UnloadedSound(string path) : base(path)
            {
            }

            public override void Load(AssetLibrary library)
            {
                library.LoadSoundEffect(this.path);
            }
        }

        private class UnloadedSpritefont : UnloadedAsset
        {
            public UnloadedSpritefont(string path) : base(path)
            {
            }

            public override void Load(AssetLibrary library)
            {
                library.LoadSpriteFont(this.path);
            }
        }

        private readonly List<UnloadedAsset> assets = new List<UnloadedAsset>();
        private readonly List<UnloadedDrawLoopAssetCallback> drawAssets = new List<UnloadedDrawLoopAssetCallback>();
        private int startingCount;

        private void AddAsset(UnloadedAsset asset)
        {
            this.assets.Add(asset);
            this.startingCount++;
        }

        private void AddDrawAsset(UnloadedDrawLoopAssetCallback asset)
        {
            this.drawAssets.Add(asset);
            this.startingCount++;
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

        public void UpdateLoadNextThing(AssetLibrary library)
        {
            if (this.assets.Count == 0)
            {
                return;
            }

            var assetToLoad = this.assets[0];
            this.assets.RemoveAt(0);
            assetToLoad.Load(library);
        }

        public void DrawLoadNextThing(AssetLibrary library, SpriteBatch spriteBatch)
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
            return this.assets.Count == 0;
        }

        public float Progress()
        {
            return 1f - (float) (this.assets.Count + this.drawAssets.Count) / this.startingCount;
        }

        public void AddMachinaAssetCallback(string assetPath, Func<IAsset> callback)
        {
            AddAsset(new UnloadedAssetCallback(assetPath, callback));
        }

        public void AddDrawAssetCallback(string assetPath, Func<SpriteBatch, IAsset> callback)
        {
            AddDrawAsset(new UnloadedDrawLoopAssetCallback(callback));
        }

        public bool IsDoneDrawLoading()
        {
            return this.drawAssets.Count == 0;
        }
    }
}