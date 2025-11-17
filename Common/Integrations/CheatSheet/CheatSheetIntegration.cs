using CheatSheet;
using Terraria.ModLoader;
using UIEditor.Core.IngameEditor.Systems;
using UIEditor.Core.LayersEditor;

namespace UIEditor.Common.Integrations.CheatSheet;

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
        texture: Ass.EditorIconSmall,
        buttonClickedAction: EditSystem.ToggleActive,
        tooltip: () => EditSystem.IsActive ? "Close UI editor panel" : "Open UI editor panel"
        );

        // Layer button
        CheatSheetInterface.RegisterButton(
        texture: Ass.LayersIconSmall,
        buttonClickedAction: LayerSystem.ToggleActive,
        tooltip: () => LayerSystem.IsActive ? "Close layer editor panel" : "Open layer editor panel"
        );
    }
}