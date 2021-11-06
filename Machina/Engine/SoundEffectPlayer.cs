namespace Machina.Engine
{
    using Data;
    using Microsoft.Xna.Framework.Audio;

    public class SoundEffectPlayer
    {
        private readonly GameSettings settings;

        public SoundEffectPlayer(GameSettings settings)
        {
            this.settings = settings;
        }

        public SoundEffectInstance PlaySound(string soundEffectName, float baseVolume = 0.5f, float pitch = 0f,
            bool useCache = true)
        {
            SoundEffectInstance soundEffect;
            if (useCache)
            {
                // Grab the cached sound effect and reset it
                soundEffect = MachinaGame.Assets.GetSoundEffectInstance(soundEffectName);
                soundEffect.Stop();
            }
            else
            {
                // Build a new sound effect here and now
                soundEffect = MachinaGame.Assets.CreateSoundEffectInstance(soundEffectName);
            }

            soundEffect.Volume = baseVolume * settings.SFXVolumeAsFloat;
            soundEffect.Pitch = pitch;
            try
            {
                soundEffect.Play();
            }
            catch (InstancePlayLimitException e)
            {
                MachinaGame.Print("Caught InstancePlayLimitException", e.Message);
            }

            return soundEffect;
        }
    }
}