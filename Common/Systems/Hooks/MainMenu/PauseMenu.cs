using Terraria;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Hooks.MainMenu
{
    public class MainMenuPauseSystem : ModSystem
    {
        // This flag will be controlled by the slider logic.
        public static bool TimeIsPausedBySlider = false;

        public override void Load()
        {
            On_Main.UpdateMenu += StopTimeInMenu;
        }

        public override void Unload()
        {
            On_Main.UpdateMenu -= StopTimeInMenu;
        }

        private void StopTimeInMenu(On_Main.orig_UpdateMenu orig)
        {
            // Log.Info(TimeIsPausedBySlider.ToString());

            //TimeIsPausedBySlider = true;

            // Only freeze time if we are in the menu AND our flag is set.
            if (Main.gameMenu && TimeIsPausedBySlider)
            {
                //Log.Info("b");
                return;
            }
            orig();
        }
    }
}