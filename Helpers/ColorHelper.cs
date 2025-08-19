using System;
using System.Globalization;
using ReLogic.OS;
using Terraria;
using Terraria.GameContent.UI.Elements;
using static Terraria.GameContent.UI.States.UICharacterCreation;

namespace UICustomizer.Helpers
{
    /// <summary>
    /// General colour utilities
    /// Callers supply any state (HSL vector, UI text, delegates) via parameters.
    /// </summary>
    public static class ColorHelper
    {
        // ─────────────────────────────────────────────────────────────
        //  Simple utility colours / math
        // ─────────────────────────────────────────────────────────────
        public static Color CalamityRed => new(226, 57, 39);
        public static Color DarkBluePanel => new(39, 49, 109);
        public static Color SuperDarkBluePanel => new(27, 29, 85);

        public static float InverseLerp(float a, float b, float v) =>
            a == b ? 0f : MathHelper.Clamp((v - a) / (b - a), 0f, 1f);

        // ─────────────────────────────────────────────────────────────
        //  Hex helpers
        // ─────────────────────────────────────────────────────────────
        public static Color HexToColor(string hex)
        {
            if (hex.StartsWith("#")) hex = hex[1..];
            if (hex.Length != 6)
                throw new ArgumentException("Hex must be 6 characters long.");

            return new Color(
                Convert.ToByte(hex[..2], 16),
                Convert.ToByte(hex.Substring(2, 2), 16),
                Convert.ToByte(hex.Substring(4, 2), 16));
        }

        public static string ColorToHex(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

        public static bool TryParseHex(string? hex, out Color c)
        {
            c = default;
            if (string.IsNullOrWhiteSpace(hex)) return false;

            try { c = HexToColor(hex); return true; }
            catch { return false; }
        }

        // ─────────────────────────────────────────────────────────────
        //  Clipboard helpers
        // ─────────────────────────────────────────────────────────────
        public static void CopyHex(Func<Color> getCurrentColor) =>
            Platform.Get<IClipboard>().Value = ColorToHex(getCurrentColor());

        /// <summary>Tries to read a colour from the clipboard and, if valid, applies it.</summary>
        public static bool PasteHex(
            ref Vector3 hsl,
            Action<Color> applyColor,
            UIText hexText = null!)
        {
            string s = Platform.Get<IClipboard>().Value.TrimStart('#');
            if (s.Length != 6 || !uint.TryParse(s, NumberStyles.HexNumber, null, out var u))
                return false;

            var c = new Color((byte)(u >> 16), (byte)(u >> 8), (byte)u);
            hsl = Main.rgbToHsl(c);
            applyColor(c);
            hexText?.SetText(ColorToHex(c));
            return true;
        }

        // ─────────────────────────────────────────────────────────────
        //  Random / slider helpers
        // ─────────────────────────────────────────────────────────────
        public static void RandomizeColor(
            ref Vector3 hsl,
            Action<Color> applyColor,
            UIText hexText = null!)
        {
            hsl = new Vector3(Main.rand.NextFloat(), Main.rand.NextFloat(), Main.rand.NextFloat());
            var c = Main.hslToRgb(hsl.X, hsl.Y, hsl.Z * 0.85f + 0.15f);
            applyColor(c);
            hexText?.SetText(ColorToHex(c));
        }

        /// <summary>
        /// Generic handler for an HSL slider edit.
        /// </summary>
        public static void ApplyHslValue(
            ref Vector3 hsl,
            HSLSliderId id,
            float value,
            Action<Color> applyColor,
            UIText hexText = null!)
        {
            if (id == HSLSliderId.Hue) hsl.X = value;
            else if (id == HSLSliderId.Saturation) hsl.Y = value;
            else hsl.Z = value;

            var c = Main.hslToRgb(hsl.X, hsl.Y, hsl.Z * 0.85f + 0.15f);
            applyColor(c);
            hexText?.SetText(ColorToHex(c));
        }
    }
}
