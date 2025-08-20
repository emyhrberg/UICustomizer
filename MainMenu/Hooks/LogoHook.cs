using System;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.ModLoader;

namespace UICustomizer.MainMenu.Hooks
{
    public class LogoHook : ModSystem
    {
        public Hook logoHook;
        public ILHook logoILHook;
        public static bool IsDrawing = true; // Whether the logo should be drawn
        public static float Scale = 1f;
        public static float Rotation = 0f;
        public static Color Color;
        public static float OffsetX = 0f; // Horizontal offset
        public static float OffsetY = 0f; // Vertical offset

        public static Texture2D CustomLogoTexture;

        public override void Load()
        {
            ApplyConfig();

            // Method
            MethodInfo m = typeof(MenuLoader).GetMethod("UpdateAndDrawModMenuInner", BindingFlags.NonPublic | BindingFlags.Static);
            if (m == null)
            {
                Log.Error("Failed to find UpdateAndDrawModMenuInner method for LogoHook.");
                return;
            }

            // Create Logo Hook
            logoHook = new Hook(
                source: m,
                target: new Action<Action<SpriteBatch, GameTime, Color, float, float>, SpriteBatch, GameTime, Color, float, float>(
                    UpdateAndDrawModMenuInner_Hook)
                );
            // Note: The 'target' in the Hook is a duplicate Action of the parameter types twice for some reason or something

            // Create IL Logo Hook
            logoILHook = new(m, ModifyLogoPos);
        }
        public override void Unload()
        {
            logoHook = null;
            logoILHook?.Dispose();
            ResetCustomLogo();
        }

        private static void ModifyLogoPos(ILContext il)
        {
            IL.Edit(il, c =>
            {
                if (!c.TryGotoNext(MoveType.After,
                        i => i.MatchNewobj<Vector2>()))
                {
                    Log.Warn("LogoHook: could not locate Vector2 ctor for logo position.");
                    return;
                }

                c.EmitDelegate<Func<Vector2, Vector2>>(pos =>
                    pos + new Vector2(OffsetX, OffsetY));
            });
        }


        private void UpdateAndDrawModMenuInner_Hook(
    Action<SpriteBatch, GameTime, Color, float, float> orig,
    SpriteBatch spriteBatch, GameTime gameTime, Color color,
    float logoRotation, float logoScale)
        {
            // 1) Skip drawing entirely if wanted
            if (!IsDrawing)
            {
                // Draw with 0 scale instead of IL hook and skipping calls
                // It's true - yes i'm lazy!
                orig(spriteBatch, gameTime, color, logoRotation, 0);
                return;
            }

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

                // Call orig with scale 0;
                // effectively drawing everything except the logo (since it's drawn at scale 0)
                orig(spriteBatch, gameTime, color, logoRotation, 0);
                return;
            }

            // 5) Finally call vanilla with whichever values survived
            orig(spriteBatch, gameTime, color, logoRotation, logoScale);
        }

        public static void ResetCustomLogo()
        {
            CustomLogoTexture = null;          // fall back to orig logo
            Conf.C.MainMenuLogo.LogoFileName = null;
            Conf.Save();
        }

        public static void ApplyConfig()
        {
            var cfg = Conf.C;

            // Load color from config
            if (ColorHelper.TryParseHex(cfg?.MainMenuLogo.Color, out var parsed))
                Color = parsed;

            // Load pos and scale from config
            Scale = cfg?.MainMenuLogo.Scale ?? 1f;
            OffsetX = cfg?.MainMenuLogo.OffsetX ?? 0f;
            OffsetY = cfg?.MainMenuLogo.OffsetY ?? 0f;
            Rotation = cfg?.MainMenuLogo.Rotation ?? 0f;

            // Load isDrawing from config
            IsDrawing = cfg?.MainMenuDraw.DrawLogo ?? true;

            // Load custom logo texture from config
            string path = cfg?.MainMenuLogo.LogoFileName;
            Log.Info($"LogoHook: Loading custom logo from path: {path}");

            if (string.IsNullOrWhiteSpace(path))
            {
                CustomLogoTexture = null;
                return;
            }

            Main.QueueMainThreadAction(() =>
            {
                CustomLogoTexture = FileUploadHelper.ReadAndCreateTextureFromPath(path);
            });
        }
    }
}
