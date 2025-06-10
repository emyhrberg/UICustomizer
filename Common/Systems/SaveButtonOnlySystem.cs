using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;
using UICustomizer.UI;
using UICustomizer.Common.Configs;
using UICustomizer.Common.States;

namespace UICustomizer.Common.Systems
{
    internal class SaveButtonOnlySystem : ModSystem
    {
        public static bool IsHideMode;
        public static Rectangle saveButtonRect;
        public SaveButtonOnlyState saveState;
        public UserInterface saveUI;

        public static void SetHideMode(bool active) => IsHideMode = active;
        public static void SetSaveButtonPosition(Rectangle rect) => saveButtonRect = rect;

        public override void Load()
        {
            // Initialize early so ModifyInterfaceLayers never sees null
            saveState = new SaveButtonOnlyState();
            saveUI = new UserInterface();
            saveUI.SetState(saveState);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (IsHideMode)
                saveUI?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int idx = layers.FindIndex(l => l.Name == "Vanilla: Mouse Text");
            if (idx == -1) return;

            layers.Insert(idx, new LegacyGameInterfaceLayer(
                "UICustomizer: SaveButtonOnly",
                () =>
                {
                    if (!IsHideMode || saveState?.saveButton == null)
                        return true;

                    var btn = saveState.saveButton;
                    btn.Left.Set(saveButtonRect.X, 0);
                    btn.Top.Set(saveButtonRect.Y, 0);
                    btn.Recalculate();

                    // Use the real draw-time GameTime
                    saveUI.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
                    return true;
                },
                InterfaceScaleType.UI
            ));
        }
    }
}