using System;
using Terraria.ModLoader;
using UICustomizer.Helpers;

namespace UICustomizer.Common.Systems.Integrations.ModReloader
{
    [JITWhenModsEnabled("ModReloader")]
    [ExtendsFromMod("DragonLens")]
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
                AddEditButton(MR);
                AddLayersButton(MR);
            }
        }

        private void AddLayersButton(Mod MR)
        {
            Log.Info("Adding AddLayersButton button for UICustomizer");
            MR.Call(
                "AddButton",
                "UI", // name
                new Action(EditorSystem.ToggleActive),
                Ass.EditorIcon, // asset
                "Edit UI layout" // tooltip
            );
            Log.Info("ModReloader AddLayersButton added for UICustomizer");
        }

        private void AddEditButton(Mod MR)
        {
            Log.Info("Adding ModReloader button for UICustomizer");
            MR.Call(
                "AddButton",
                "UI", // name
                new Action(EditorSystem.ToggleActive),
                Ass.EditorIcon, // asset
                "Edit UI layout" // tooltip
            );
            Log.Info("ModReloader button added for UICustomizer");
        }
    }
}