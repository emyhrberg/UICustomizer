using System;
using Terraria.ModLoader;

namespace UICustomizer.Common.Systems.Integrations.Heros
{
    [JITWhenModsEnabled("HEROsMod")]
    public sealed class HerosModIntegration : ModSystem
    {
        private const string EditPermissionKey = "Editor";
        private const string LayerPermissionKey = "Layer";

        public override void PostSetupContent()
        {
            if (ModLoader.TryGetMod("HEROsMod", out Mod herosMod))
            {
                AddButtons(herosMod);
            }
        }

        private void AddButtons(Mod herosMod)
        {
            // Add editor permission
            // herosMod.Call("AddPermission",
            // EditPermissionKey, // permission Name
            // Loc.Get("EditorPanel.Icon.Label"), // header
            // (bool hasPerm) => PermissionChanged(hasPerm, EditPermissionKey)); // groupUpdated

            // Add editor button
            herosMod.Call("AddSimpleButton",
                EditPermissionKey, // permission Name
                Ass.EditorIcon, // icon
                () => EditorSystem.ToggleActive(),
                (Action<bool>)(hasPerm => PermissionChanged(hasPerm, EditPermissionKey)), // permission changed
                () => EditorSystem.IsActive ? Loc.Get("EditorPanel.Icon.Close") : Loc.Get("EditorPanel.Icon.Open") // tooltip
            );

            // Add layer permission
            // herosMod.Call("AddPermission",
            // LayerPermissionKey, // permission Name
            // Loc.Get("LayerPanel.Icon.Label"), // header
            // (bool hasPerm) => PermissionChanged(hasPerm, LayerPermissionKey)); // groupUpdated

            // Add layer button
            herosMod.Call("AddSimpleButton",
                LayerPermissionKey, // permission Name
                Ass.LayersIcon, // icon
                () => LayerSystem.ToggleActive(),
                (Action<bool>)(hasPerm => PermissionChanged(hasPerm, LayerPermissionKey)), // permission changed
                () => LayerSystem.IsActive ? Loc.Get("LayerPanel.Icon.Close") : Loc.Get("LayerPanel.Icon.Open") // tooltip
            );
        }

        private static void PermissionChanged(bool hasPerm, string permissionName)
        {
            if (!hasPerm)
            {
                //Main.NewText($"⛔ You lost permission to use the {permissionName} button!", ColorHelper.CalamityRed);
                Log.Info($"You lost permission for {permissionName} button. You cannot use it anymore.");
            }
            else
            {
                //Main.NewText($"✅ You regained permission to use the {permissionName} button!", Color.LightGreen);
                Log.Info($"You regained permission for {permissionName} button. You can use it again.");
            }
        }
    }
}
