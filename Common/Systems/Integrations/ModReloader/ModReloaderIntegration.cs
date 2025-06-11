using System;
using Terraria.ModLoader;
using UICustomizer.Helpers;

namespace UICustomizer.Common.Systems.Integrations.ModReloader
{
    [JITWhenModsEnabled("ModReloader")]
    public sealed class ModReloaderIntegration : ModPlayer
    {
        public override void OnEnterWorld()
        {
            Log.Info("ModReloaderIntegration running...");

            if (ModLoader.TryGetMod("DragonLens", out Mod _))
            {
                return;
            }

            if (ModLoader.TryGetMod("ModReloader", out Mod MR))
            {
                Log.Info("ModReloader foundz...");
                AddButtons(MR);
            }
        }

        private void AddButtons(Mod MR)
        {
            Log.Info("Adding UI button to MR...");
            MR.Call(
                "AddButton",
                "Edit", // name
                () => EditorSystem.ToggleActive(),
                Ass.EditorIcon, // asset
                "Edit Element positions, Drag and drop, Save Layouts!" // tooltip
            );

            Log.Info("Adding Layers button to MR...");
            MR.Call(
                "AddButton",
                "Layers", // name
                () => LayerSystem.ToggleActive(),
                Ass.LayersIcon, // asset
                "Toggle UIElements, Drawn Interface Layers from all mods, and Toggle Resource Packs directly in-game." // tooltip
            );
        }
    }
}