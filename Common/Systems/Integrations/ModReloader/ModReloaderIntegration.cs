using System;
using Terraria.GameContent;

namespace UICustomizer.Common.Systems.Integrations.ModReloader
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
                AddMRButton(MR);
            }
        }

        private void AddMRButton(Mod MR)
        {
            Log.Info("Adding ModReloader button for UICustomizer");
            MR.Call(
                "AddButton",
                "UI", // name
                new Action(UICustomizerSystem.ToggleEditMode),
                Ass.DragonLensToolIcon, // asset
                "Edit UI layout" // tooltip
            );
            Log.Info("ModReloader button added for UICustomizer");
        }
    }
}