using System;
using System.Reflection;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.ModLoader;

namespace UICustomizer.Core.MainMenuEditor.Hooks
{
    public class SkipCloudsHook : ModSystem
    {
        public static bool IsDrawing = true;

        private Hook _cloudHook;

        public override void Load()
        {
            // local lambda: DrawSurfaceBG -> g__DrawCloud
            MethodInfo m = typeof(Main).GetMethod(
                $"<{nameof(Main.DrawSurfaceBG)}>g__DrawCloud|1826_0",
                BindingFlags.NonPublic | BindingFlags.Static);

            if (m == null)
            {
                Log.Error("DrawSurfaceBG → DrawCloud lambda not found – cloud hook skipped");
                return;
            }

            _cloudHook = new Hook(
                m,
                new Action<Action<int, Color, float>, int, Color, float>((orig, idx, col, yOff) =>
                {
                    if (!IsDrawing) return;
                    orig(idx, col, yOff);
                }));
        }

        public override void Unload() => _cloudHook?.Dispose();
    }
}
