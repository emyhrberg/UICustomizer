using Terraria;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Hooks.MainMenu
{
    public class SkipBackgroundDrawHook : ModSystem
    {
        public static bool IsDrawing = true;
        public override void Load()
        {
            On_Main.DrawBackground += SkipDraw;
        }

        public override void Unload()
        {
            On_Main.DrawBackground -= SkipDraw;
        }
        private void SkipDraw(On_Main.orig_DrawBackground orig, Main self)
        {
            if (IsDrawing)
            {
                orig(self);
            }
        }
    }
}