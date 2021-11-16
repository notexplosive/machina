using Microsoft.Xna.Framework;

namespace Machina.Data
{
    public class GameSettings
    {
        public readonly Point startingWindowSize;

        public virtual float SFXVolumeAsFloat => 1f;

        public GameSettings(Point startingWindowSize)
        {
            this.startingWindowSize = startingWindowSize;
        }

        public virtual void LoadSavedSettingsIfExist()
        {
        }
    }
}