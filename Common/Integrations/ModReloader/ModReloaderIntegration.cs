using Terraria.ModLoader;
using UIEditor.Core.IngameEditor.Systems;
using UIEditor.Core.LayersEditor;

namespace UIEditor.Common.Integrations.ModReloader;

[JITWhenModsEnabled("ModReloader")]
public sealed class ModReloaderIntegration : ModPlayer
{
    public override void OnEnterWorld()
    {
        if (ModLoader.TryGetMod("DragonLens", out Mod _))
            return;
        if (ModLoader.TryGetMod("HEROsMod", out Mod _))
            return;
        if (ModLoader.TryGetMod("CheatSheet", out Mod _))
            return;

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
            () => EditSystem.ToggleActive(),
            Ass.EditorIcon, // asset
            "Edit UI layout" // tooltip
        );
        MR.Call(
            "AddButton",
            "Layers", // name
            () => LayerSystem.ToggleActive(),
            Ass.LayersIcon, // asset
            "Edit UI layout" // tooltip
        );
        Log.Info("Added ModReloader button for UIEditor");
    }
}