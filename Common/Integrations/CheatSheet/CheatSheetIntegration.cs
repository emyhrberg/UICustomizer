using CheatSheet;
using Terraria.ModLoader;
using UICustomizer.Core.IngameEditor.System;

namespace UICustomizer.Common.Integrations.CheatSheet
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
            var sys = ModContent.GetInstance<EditSystem>();

            // Edit button
            CheatSheetInterface.RegisterButton(
            texture: Ass.EditorIconSmall,
            buttonClickedAction: sys.Toggle,
            tooltip: () => sys.Enabled ? Loc.Get("EditorPanel.Icon.Close") : Loc.Get("EditorPanel.Icon.Open")
            );
        }
    }
}