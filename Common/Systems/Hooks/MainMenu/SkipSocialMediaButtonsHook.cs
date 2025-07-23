using Terraria;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Hooks.MainMenu
{
    public class SkipSocialMediaButtonsHook : ModSystem
    {
        public static bool IsDrawing = true;

        public override void Load()
        {
            On_Main.DrawSocialMediaButtons += SkipDraw;
            On_Main.DrawtModLoaderSocialMediaButtons += SkipDraw2;
        }

        public override void Unload()
        {
            On_Main.DrawSocialMediaButtons -= SkipDraw;
            On_Main.DrawtModLoaderSocialMediaButtons -= SkipDraw2;
        }

        private void SkipDraw(On_Main.orig_DrawSocialMediaButtons orig, Color menuColor, float upBump)
        {
            if (IsDrawing)
            {
                orig(menuColor, upBump);
            }
        }

        private void SkipDraw2(On_Main.orig_DrawtModLoaderSocialMediaButtons orig, Color menuColor, float upBump)
        {
            if (IsDrawing)
            {
                orig(menuColor, upBump);
            }
        }
    }
}