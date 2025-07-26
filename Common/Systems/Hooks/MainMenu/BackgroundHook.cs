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

        // user-tweakable transforms
        public static float Scale = 1f;      // 1 = fill screen
        public static float Rotation = 0f;     // radians
        public static Color Color = Color.White;
        public static float OffsetX = 0f;
        public static float OffsetY = 0f;

        private Hook _drawBGHook;

        // ─────────────────────────── HOOK ────────────────────────────
        public override void Load()
        {
            MethodInfo m = typeof(Main).GetMethod("DrawBG",
                              BindingFlags.Instance | BindingFlags.NonPublic);
            if (m == null)
            {
                Log.Warn("Main.DrawBG not found – background hook disabled.");
                return;
            }

            // DrawBG(GameTime) detour
            if (m != null)
            {
                _drawBGHook = new Hook(m,
                    new Action<Action<Main>, Main>((orig, self) =>
                    {
                        Detour(orig, self);
                    }));

                _drawBGHook = new Hook(m, new Action<Action<Main>, Main>(Detour));
            }
        }

        public override void Unload()
        {
            _drawBGHook?.Dispose();
            ResetCustomBackground();
        }

        private static void Detour(Action<Main> orig, Main self)
        {
            if (!IsDrawing)
                return;                     // nothing at all

            if (CustomBackgroundTexture is null)
            {
                orig(self);            // vanilla sky
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

            Main.spriteBatch.Begin(SpriteSortMode.Deferred,
                                   BlendState.AlphaBlend,
                                   SamplerState.LinearClamp,
                                   DepthStencilState.None,
                                   RasterizerState.CullCounterClockwise,
                                   null,
                                   Matrix.Identity);

            Main.spriteBatch.Draw(tex,
                                  pos,
                                  null,
                                  Color,
                                  Rotation,
                                  origin,
                                  drawScale,
                                  SpriteEffects.None,
                                  0f);

            Main.spriteBatch.End();
        }

        // ───────────────────── helper for your UI ────────────────────
        public static void LoadCustomBackground(string path)
        {
            ResetCustomBackground();

            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                return;

            using FileStream fs = File.OpenRead(path);
            CustomBackgroundTexture =
                Texture2D.FromStream(Main.graphics.GraphicsDevice, fs);

            // reset transforms
            Scale = 1f;
            Rotation = 0f;
            Color = Color.White;
            OffsetX = OffsetY = 0f;
        }

        public static void ResetCustomBackground()
        {
            if (CustomBackgroundTexture is { IsDisposed: false })
                CustomBackgroundTexture.Dispose();
            CustomBackgroundTexture = null;
        }
    }
}
