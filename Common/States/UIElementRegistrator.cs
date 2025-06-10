using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using UICustomizer.Helpers;

namespace UICustomizer.Common.States
{
    /// <summary>
    /// Detects, registrates and keeps track of all UIElements drawn in the hook UIElement.Draw().
    /// </summary>
    public class UIElementRegistrator : ModSystem
    {
        public override void Load()
        {
            // On_UIElement.Draw += ElementDrawingHook;
        }

        //private readonly Dictionary<string, string> modElements;
        public static List<string> allElements = [];

        public void ElementDrawingHook(On_UIElement.orig_Draw orig, UIElement self, SpriteBatch spriteBatch)
        {
            orig(self, spriteBatch); // Normal UI behavior
            return;

            // Do some invalid checks
            if (Main.dedServ || Main.gameMenu)
                return;
            //if (self.GetOuterDimensions().Width > 900 || self.GetOuterDimensions().Height > 900)
            //    return;

            // Get the element names and register them
            string elementTypeName = self.GetType().Name;
            Mod modInst = Helpers.ModUtils.GetModInstance(self.GetType());
            //Log.Info("modInst:" + modInst.Name.ToString());

            if (!allElements.Contains(elementTypeName))
            {
                //Log.Info("Found UI element: " + elementTypeName + " in mod: " + (modInst?.Name ?? "Unknown"));
                allElements.Add(elementTypeName);
            }
        }
    }
}