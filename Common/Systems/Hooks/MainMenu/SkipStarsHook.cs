using Terraria;
using Terraria.ModLoader;
using static Terraria.Main;

namespace UICustomizer.Common.Systems.Hooks.MainMenu
{
    public class SkipStarsHook : ModSystem
    {
        public static bool IsDrawing = true;

        public override void Load()
        {
            // Load isDrawing from config
            IsDrawing = Conf.C?.MainMenuDraw.DrawStars ?? true;
            On_Main.DrawStarsInBackground += SkipDraw;
        }
        public override void Unload()
        {
            On_Main.DrawStarsInBackground -= SkipDraw;
        }
        private void SkipDraw(On_Main.orig_DrawStarsInBackground orig, Main self, SceneArea sceneArea, bool artificial)
        {
            if (!IsDrawing) return;

            orig(self, sceneArea, artificial);
        }
    }
}