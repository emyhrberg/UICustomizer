using System;

namespace UICustomizer.Helpers
{
    // Also see:
    // UICommon.DefaultBlue
    // UICommon.MainPanelBackground
    // Main.OurFavoriteColor (yellow)

    public static class ColorHelper
    {
        public static Color CalamityRed => new(226, 57, 39);
        public static Color DarkBluePanel = new(39, 49, 109);
        public static Color SuperDarkBluePanel = new(27, 29, 85);

        public static Color HexToColor(string hex)
        {
            if (hex.StartsWith("#"))
                hex = hex[1..]; // remove '#'

            if (hex.Length != 6)
                throw new ArgumentException("Hex must be 6 characters long.");

            byte r = Convert.ToByte(hex.Substring(0, 2), 16);
            byte g = Convert.ToByte(hex.Substring(2, 2), 16);
            byte b = Convert.ToByte(hex.Substring(4, 2), 16);

            return new Color(r, g, b);
        }
        public static float InverseLerp(float a, float b, float value)
        {
            if (a == b) return 0f;
            return MathHelper.Clamp((value - a) / (b - a), 0f, 1f);
        }

        public static Color TryParseHex(string hex, Color fallback)
        {
            if (string.IsNullOrWhiteSpace(hex))
                return fallback;

            try
            {
                return HexToColor(hex);
            }
            catch
            {
                return fallback;
            }
        }
    }
}