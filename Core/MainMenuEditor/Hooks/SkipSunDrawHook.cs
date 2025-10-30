
using Terraria;
using Terraria.ModLoader;
using static Terraria.Main;

namespace UICustomizer.Core.MainMenuEditor.Hooks
{
    public class SkipSunDrawHook : ModSystem
    {
        public static bool IsDrawing = true;

        public override void Load()
        {
            // Load isDrawing from config
            IsDrawing = Conf.C?.MainMenuDraw.DrawSun ?? true;
            On_Main.DrawSunAndMoon += SkipDraw;
        }
        public override void Unload()
        {
            On_Main.DrawSunAndMoon -= SkipDraw;
        }
        private void SkipDraw(On_Main.orig_DrawSunAndMoon orig, Main self, SceneArea sceneArea, Color moonColor, Color sunColor, float tempMushroomInfluence)
        {
            if (!IsDrawing) return;

            orig(self, sceneArea, moonColor, sunColor, tempMushroomInfluence);
        }
    }
}
