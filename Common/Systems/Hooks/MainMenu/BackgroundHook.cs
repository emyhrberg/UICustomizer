using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Hooks.MainMenu
{
    /// <summary>Overlays the main-menu background with a custom image.</summary>
    public sealed class BackgroundHook : ModSystem
    {
        public static bool IsDrawing = true;
        public static Texture2D? CustomBackgroundTexture;

        public static float Scale = 1f;
        public static float Rotation = 0f;
        public static Color Color = Color.White;
        public static float OffsetX = 0f;
        public static float OffsetY = 0f;

        private Hook _drawBGHook;

        public override void Load()
        {
            // Load color from config
            if (ColorHelper.TryParseHex(Conf.C.MainMenuBackground.Color, out var color))
            {
                Color = color;
            }

            // Load pos and scale from config
            var cfg = Conf.C;
            Scale = cfg?.MainMenuBackground.Scale ?? 1f;
            OffsetX = cfg?.MainMenuBackground.OffsetX ?? 0f;
            OffsetY = cfg?.MainMenuBackground.OffsetY ?? 0f;
            Rotation = cfg?.MainMenuBackground.Rotation ?? 0f;

            IsDrawing = cfg?.MainMenuDraw.DrawBackground ?? true;

            // Load path from config
            string path = Conf.C.MainMenuBackground.BackgroundFileName;
            Log.Info($"BackgroundHook: Loading custom background from path: {path}");

            if (!string.IsNullOrEmpty(path))
            {
                Main.QueueMainThreadAction(() =>
                {
                    CustomBackgroundTexture =
                        FileUploadHelper.ReadAndCreateTextureFromPath(path);
                });
            }

            // Create and run the patching of the DrawBG hook
            MethodInfo m = typeof(Main).GetMethod("DrawBG", BindingFlags.Instance | BindingFlags.NonPublic);

            if (m != null)
            {
                _drawBGHook = new Hook(m, new Action<Action<Main>, Main>(DrawBGDetour));
            }
        }

        public override void Unload()
        {
            _drawBGHook?.Dispose();
            ResetCustomBackground();
        }

        private static void DrawBGDetour(Action<Main> orig, Main self)
        {
            if (!IsDrawing)
                return;

            if (CustomBackgroundTexture is null)
            {
                orig(self);
                return;
            }

            // ── draw the custom texture full-screen ──────────────────
            Texture2D tex = CustomBackgroundTexture;

            float fit = MathF.Max((float)Main.screenWidth / tex.Width,
                                  (float)Main.screenHeight / tex.Height);
            float drawScale = fit * Scale;

            Vector2 origin = new(tex.Width * 0.5f, tex.Height * 0.5f);
            Vector2 pos = new(Main.screenWidth * 0.5f + OffsetX,
                                 Main.screenHeight * 0.5f + OffsetY);

            Main.spriteBatch.Draw(tex, pos, null, Color, Rotation, origin, drawScale, SpriteEffects.None, 0f);
        }

        public static void ResetCustomBackground()
        {
            CustomBackgroundTexture = null;
            Conf.C.MainMenuBackground.BackgroundFileName = null;
            Conf.Save();
        }
    }
}
