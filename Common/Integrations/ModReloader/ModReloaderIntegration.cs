using Terraria.ModLoader;
using UICustomizer.EditMode.System;

namespace UICustomizer.Common.Integrations.ModReloader
{
    [JITWhenModsEnabled("ModReloader")]
    public sealed class ModReloaderIntegration : ModPlayer
    {
        public override void OnEnterWorld()
        {
            if (ModLoader.TryGetMod("DragonLens", out Mod _))
            {
                return;
            }

            if (ModLoader.TryGetMod("ModReloader", out Mod MR))
            {
                AddButtons(MR);
            }
        }

        private void AddButtons(Mod MR)
        {
            MR.Call(
                "AddButton",
                "UI", // name
                () => ModContent.GetInstance<EditSystem>().Toggle,
                Ass.EditorIconSmall, // asset
                "Edit UI layout" // tooltip
            );
            Log.Info("Added ModReloader button for UICustomizer");
        }
    }
}