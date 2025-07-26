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
        public static bool IsDrawing = true; // Whether the logo should be drawn
        public static float Scale = 1f;
        public static float Rotation = 0f;
        public static Color Color;
        public static float OffsetX = 0f; // Horizontal offset
        public static float OffsetY = 0f; // Vertical offset

        public static Texture2D? CustomLogoTexture;

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

            // Note: The 'target' in the Hook is a duplicate Action of the parameter types twice for some reason or something
        }

        private void UpdateAndDrawModMenuInner_Hook(
    Action<SpriteBatch, GameTime, Color, float, float> orig,
    SpriteBatch spriteBatch, GameTime gameTime, Color color,
    float logoRotation, float logoScale)
        {
            orig(spriteBatch, gameTime, color, logoRotation, logoScale);

            // 1) Skip drawing entirely if wanted
            if (!IsDrawing) return;          // keep the fast‑return

            // 2) Rotation override (keep vanilla otherwise)
            if (Math.Abs(Rotation) > float.Epsilon)          // ≠ 0
                logoRotation = Rotation;

            // 3) Scale override (keep vanilla otherwise)
            if (Math.Abs(Scale - 1f) > float.Epsilon)        // ≠ 1
                logoScale = Scale;

            // 4) Colour override (use alpha‑0 as "no override")
            if (Color.A != 0)
                color = Color;

            // if the user loaded a texture, draw it manually and skip the vanilla logo.
            if (CustomLogoTexture is not null)
            {
                var pos = new Vector2(Main.screenWidth / 2f, 100f);
                pos += new Vector2(OffsetX, OffsetY);
                var origin = new Vector2(CustomLogoTexture.Width / 2f,
                                         CustomLogoTexture.Height / 2f);

                spriteBatch.Draw(CustomLogoTexture,
                                 position: pos,
                                 sourceRectangle: null,
                                 color: Color == default ? Color.White : Color,
                                 rotation: Rotation,
                                 origin: origin,
                                 scale: Scale,
                                 effects: SpriteEffects.None,
                                 layerDepth: 0f);
                return; // done – do NOT call orig()
            }

            // 5) Finally call vanilla with whichever values survived
        }
        
        
        
        
        public static void ResetCustomLogo()
        {
            // free the texture if we loaded one
            if (CustomLogoTexture != null && !CustomLogoTexture.IsDisposed)
                CustomLogoTexture.Dispose();

            CustomLogoTexture = null;          // fall back to orig logo
        }

    }
}
