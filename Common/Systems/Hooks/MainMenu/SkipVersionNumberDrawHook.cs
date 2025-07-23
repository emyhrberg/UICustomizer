using Terraria;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Hooks.MainMenu
{
    public class SkipVersionNumberDrawHook : ModSystem
    {
        public static bool IsDrawing = true;

        public override void Load()
        {
            On_Main.DrawVersionNumber += SkipDraw;
        }

        public override void Unload()
        {
            On_Main.DrawVersionNumber -= SkipDraw;
        }

        private void SkipDraw(On_Main.orig_DrawVersionNumber orig, Color menuColor, float upBump)
        {
            if (IsDrawing)
            {
                orig(menuColor, upBump);
            }
        }
    }
}