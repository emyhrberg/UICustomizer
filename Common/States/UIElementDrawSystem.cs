using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace UICustomizer.Common.States
{
    /// <summary>
    /// Detects, registrates and keeps track of all UIElements drawn in the hook UIElement.Draw().
    /// </summary>
    public class UIElementDrawSystem : ModSystem
    {
        public override void Load()
        {
            On_UIElement.Draw += ElementDrawingHook;
        }

        public override void Unload()
        {
            On_UIElement.Draw -= ElementDrawingHook;
            modElementMap.Clear();
        }

        // Key: Mod name,
        // Value: List of UI element type names (FullName for uniqueness)
        public static Dictionary<string, List<string>> modElementMap = [];

        public static Dictionary<string, bool> elementVisibilityStates = [];

        public static void ElementDrawingHook(On_UIElement.orig_Draw orig, UIElement self, SpriteBatch spriteBatch)
        {
            // If the element is not visible, skip drawing
            string elementTypeName = self.GetType().FullName;
            if (elementVisibilityStates.TryGetValue(elementTypeName, out bool isVisible) && !isVisible)
                return;

            orig(self, spriteBatch); // Normal UI behavior

            // Do some invalid checks
            if (Main.dedServ || Main.gameMenu)
                return;
            //if (self.GetOuterDimensions().Width > 900 || self.GetOuterDimensions().Height > 900)
            //    return;

            // Get the element names and register them
            Mod modInst = Helpers.ModUtils.GetModInstance(self.GetType());

            string modName = modInst?.Name ?? "Unknown"; // Default to "Unknown" if mod instance is null (for vanilla elements)

            // Check if the mod is already in our map
            if (!modElementMap.TryGetValue(modName, out List<string> elementsInMod))
            {
                elementsInMod = [];
                modElementMap[modName] = elementsInMod;
            }

            // Add the element to the mod's list if it's not already there
            if (!elementsInMod.Contains(elementTypeName))
            {
                elementsInMod.Add(elementTypeName);
                Log.Info($"Registered UI Element: {modName}: {elementTypeName} (count: {elementsInMod.Count})");
            }
        }
    }
}