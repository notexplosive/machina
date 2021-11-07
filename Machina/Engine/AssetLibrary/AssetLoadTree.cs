namespace Machina.Engine.AssetLibrary
{
    using System.Collections.Generic;

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
        private int startingCount;

        private void AddAsset(UnloadedAsset asset)
        {
            this.assets.Add(asset);
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

        public void LoadNextThing(AssetLibrary library)
        {
            var assetToLoad = this.assets[0];
            this.assets.RemoveAt(0);
            assetToLoad.Load(library);
        }

        public bool IsDoneLoading()
        {
            return this.assets.Count == 0;
        }

        public void LoadEverythingAtOnce(AssetLibrary library)
        {
            while (!IsDoneLoading())
            {
                LoadNextThing(library);
            }
        }

        public float Progress()
        {
            return 1f - (float) this.assets.Count / this.startingCount;
        }
    }
}