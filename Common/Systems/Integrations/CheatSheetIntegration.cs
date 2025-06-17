using CheatSheet;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Integrations
{
    [JITWhenModsEnabled("CheatSheet")]
    public class CheatSheetIntegration : ModSystem
    {
        public override void PostSetupContent()
        {
            if (ModLoader.TryGetMod("HEROsMod", out Mod _))
            {
                Log.Info("HEROsMod is loaded, skipping CheatSheet integration.");
                return;
            }

            // Only run if CheatSheet is loaded and we’re on client
            if (ModLoader.TryGetMod("CheatSheet", out Mod _))
            {
                // It's a good idea to extract the method here, to avoid it being called when the mod is not loaded. Otherwise, it might throw an error.
                AddButtons();
            }
        }

        private void AddButtons()
        {
            // Edit button
            CheatSheetInterface.RegisterButton(
            texture: Ass.EditorIcon,
            buttonClickedAction: EditorSystem.ToggleActive,
            tooltip: () => EditorSystem.IsActive ? Loc.Get("EditorPanel.Icon.Close") : Loc.Get("EditorPanel.Icon.Open")
            );

            // Layers button
            CheatSheetInterface.RegisterButton(
            texture: Ass.LayersIcon,
            buttonClickedAction: LayerSystem.ToggleActive,
            tooltip: () => LayerSystem.IsActive ? Loc.Get("LayerPanel.Icon.Close") : Loc.Get("LayerPanel.Icon.Open")
            );
        }
    }
}
