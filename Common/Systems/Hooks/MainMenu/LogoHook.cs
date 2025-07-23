using System;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Hooks.MainMenu
{
    public class LogoHook : ModSystem
    {
        public Hook logoHook;
        public static float LogoScale = 1f;
        public static float LogoRotation = 0f;
        public static Color LogoColor;

        public override void Load() => CreateLogoHook();
        public override void Unload() => logoHook = null;

        private void CreateLogoHook()
        {
            // Method
            MethodInfo method = typeof(MenuLoader).GetMethod("UpdateAndDrawModMenuInner", BindingFlags.NonPublic | BindingFlags.Static);
            if (method == null)
            {
                Log.Error("Failed to find UpdateAndDrawModMenuInner method for LogoHook.");
                return;
            }

            // Hook
            logoHook = new Hook(
                source: method,
                target: new Action<Action<SpriteBatch, GameTime, Color, float, float>, SpriteBatch, GameTime, Color, float, float>(
                    UpdateAndDrawModMenuInner_Hook)
                );

            // Note: The target is a duplicate Action of the parameter types twice for some reason or something
        }

        private void UpdateAndDrawModMenuInner_Hook(
            Action<SpriteBatch, GameTime, Color, float, float> orig,
            SpriteBatch spriteBatch, GameTime gameTime, Color color, float logoRotation, float logoScale)
        {
            if (MenuLoader.CurrentMenu is ModMenu menu)
            {
                logoRotation = LogoHook.LogoRotation;
                logoScale = LogoHook.LogoScale;
                color = LogoHook.LogoColor;
            }

            Color test = Color.Red;
            orig(spriteBatch, gameTime, test, logoRotation, logoScale);
        }
    }
}
