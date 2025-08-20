using System;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.ModLoader;

namespace UICustomizer.MainMenu.Hooks
{
    /// <summary>Overlays the main-menu background with a custom image.</summary>
    public sealed class BackgroundHook : ModSystem
    {
        public static bool IsDrawing = true;
        public static Texture2D CustomBackgroundTexture;

        public static float Scale = 1f;
        public static float Rotation = 0f;
        public static Color Color = Color.White;
        public static float OffsetX = 0f;
        public static float OffsetY = 0f;

        private Hook _drawBGHook;

        public override void Load()
        {
            // Create background texture from config, if available
            if (Conf.C?.MainMenuBackground.BackgroundFileName is not null)
            {
                CustomBackgroundTexture = FileUploadHelper.ReadAndCreateTextureFromPath(Conf.C.MainMenuBackground.BackgroundFileName);
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

        public static void ApplyConfig(Config cfg)
        {
            // Load color from config (default stays if parse fails)
            if (ColorHelper.TryParseHex(cfg?.MainMenuBackground.Color, out var parsed))
                Color = parsed;

            // Load transform from config
            Scale = cfg?.MainMenuBackground.Scale ?? 1f;
            OffsetX = cfg?.MainMenuBackground.OffsetX ?? 0f;
            OffsetY = cfg?.MainMenuBackground.OffsetY ?? 0f;
            Rotation = cfg?.MainMenuBackground.Rotation ?? 0f;
            IsDrawing = cfg?.MainMenuDraw.DrawBackground ?? true;

            // (Re)load custom background texture from path if provided; clear if empty
            string path = cfg?.MainMenuBackground.BackgroundFileName;
            Log.Info($"BackgroundHook: Loading custom background from path: {path}");

            if (string.IsNullOrWhiteSpace(path))
            {
                CustomBackgroundTexture = null;
                return;
            }

            Main.QueueMainThreadAction(() =>
            {
                CustomBackgroundTexture = FileUploadHelper.ReadAndCreateTextureFromPath(path);
            });
        }
    }
}
