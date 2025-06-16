using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;

namespace UICustomizer.Common.Systems
{
    internal class DarkSystem : ModSystem
    {
        #region Dark Mode Overlay
        private static float DarknessLevel = 0.0f;
        public static void SetDarknessLevel(float num) => DarknessLevel = num*0.01f;
        public static float GetDarknessLevel() => DarknessLevel;

        private static void DrawDarkOverlay()
        {
            // Draw a dark overlay covering the entire screen with the given darkness level
            Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * DarknessLevel);
        }

        #endregion

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            // Dark mode overlay at bottom
            // https://github.com/tModLoader/tModLoader/wiki/Vanilla-Interface-layers-values
            int firstVanillaLayer = layers.FindIndex(layer => layer.Name == "Vanilla: Interface Logic 1");
            if (firstVanillaLayer != -1)
            {
                layers.Insert(firstVanillaLayer, new LegacyGameInterfaceLayer(
                    "UICustomizer: Dark",
                    () =>
                    {
                        DrawDarkOverlay();
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}