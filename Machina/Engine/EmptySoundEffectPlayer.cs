using Machina.Data;
using Microsoft.Xna.Framework.Audio;

namespace Machina.Engine
{
    internal class EmptySoundEffectPlayer : ISoundEffectPlayer
    {
        public EmptySoundEffectPlayer()
        {
        }

        public SoundEffectInstance PlaySound(string soundEffectName, float baseVolume = 0.5F, float pitch = 0, bool useCache = true)
        {
            // do nothing on purpose
            return null;
        }
    }
}