using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Hooks.MainMenu
{
    public class SkipCloudsHook : ModSystem
    {
        public static bool IsDrawing = true;

        private Hook _cloudHook;

        public override void Load()
        {
            // private static void DrawCloud(int, Color, float)
            var m = typeof(Main).GetMethod(
    "DrawCloud",
    BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
    null,
    new[] { typeof(int), typeof(Color), typeof(float) },
    null);
            if (m == null)
            {
                Log.Error("Main.DrawCloud not found – cloud hook skipped");
                return;
            }

            _cloudHook = new Hook(m,
                new Action<Action<int, Color, float>, int, Color, float>((orig, idx, col, yOff) => {
                    if (IsDrawing)
                        orig(idx, col, yOff);          // vanilla cloud draw
                }));
        }

        public override void Unload() => _cloudHook?.Dispose();
    }
}
