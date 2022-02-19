using Microsoft.Xna.Framework;

namespace Machina.Data
{
    public static class ColorExtensions
    {
        public static Color WithMultipliedOpacity(this Color color, float opacity)
        {
            var resultColor = new Color(color, opacity);

            resultColor.R = (byte) (resultColor.R * opacity);
            resultColor.G = (byte) (resultColor.G * opacity);
            resultColor.B = (byte) (resultColor.B * opacity);

            return resultColor;
        }
    }
}
