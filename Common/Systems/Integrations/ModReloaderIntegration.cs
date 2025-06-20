using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Integrations
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
            Log.Info("Adding AddLayersButton button for UICustomizer");
            MR.Call(
                "AddButton",
                "Layers", // name
                () => LayerSystem.ToggleActive(),
                Ass.LayersIcon, // asset
                "Toggle UIElements, Drawn Interface Layers from all mods, and Toggle Resource Packs directly in-game." // tooltip
            );
            Log.Info("ModReloader AddLayersButton added for UICustomizer");

            Log.Info("Adding ModReloader button for UICustomizer");
            MR.Call(
                "AddButton",
                "UI", // name
                () => EditorSystem.ToggleActive(),
                Ass.EditorIcon, // asset
                "Edit UI layout" // tooltip
            );
            Log.Info("ModReloader button added for UICustomizer");
        }
    }
}