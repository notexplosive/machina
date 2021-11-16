namespace Machina.Data
{
    public class GameSettings
    {
        public virtual float SFXVolumeAsFloat => 1f;

        public virtual void LoadSavedSettingsIfExist()
        {
        }
    }
}