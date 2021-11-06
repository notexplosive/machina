namespace Machina.Engine.AssetLibrary
{
    using Data;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Media;

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
        public SoundEffect GetSoundEffect(string name);
        public Song GetSong(string name);
    }
}