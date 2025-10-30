using Terraria;
using Terraria.ModLoader;

namespace UICustomizer.Core.MainMenuEditor.Hooks
{
    public class SkipVersionNumberDrawHook : ModSystem
    {
        public static bool IsDrawing = true;

        public override void Load()
        {
            // Load isDrawing from config
            IsDrawing = Conf.C?.MainMenuDraw.DrawVersion ?? true;
            On_Main.DrawVersionNumber += SkipDraw;
            On_Main.DrawSocialMediaButtons += SkipDraw2;
            On_Main.DrawtModLoaderSocialMediaButtons += SkipDraw3;
        }

        public override void Unload()
        {
            On_Main.DrawVersionNumber -= SkipDraw;
            On_Main.DrawSocialMediaButtons -= SkipDraw2;
            On_Main.DrawtModLoaderSocialMediaButtons -= SkipDraw3;
        }

        private void SkipDraw(On_Main.orig_DrawVersionNumber orig, Color menuColor, float upBump)
        {
            if (IsDrawing) orig(menuColor, upBump);
        }

        private void SkipDraw2(On_Main.orig_DrawSocialMediaButtons orig, Color menuColor, float upBump)
        {
            if (IsDrawing) orig(menuColor, upBump);
        }

        private void SkipDraw3(On_Main.orig_DrawtModLoaderSocialMediaButtons orig, Color menuColor, float upBump)
        {
            if (IsDrawing) orig(menuColor, upBump);
        }
    }
}