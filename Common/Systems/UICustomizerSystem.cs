using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.UI;

namespace UICustomizer.Common.Systems
{
    [Autoload(Side = ModSide.Client)]
    internal class UICustomizerSystem : ModSystem
    {
        // Handle edit mode state.
        public static bool EditModeActive { get; private set; } = false;
        public static void EnterEditMode()
        {
            ModContent.GetInstance<UICustomizerSystem>().uiCustomizerState.saveButton.Active = true;
            ModContent.GetInstance<UICustomizerSystem>().uiCustomizerState.cancelButton.Active = true;
            EditModeActive = true;
            Main.NewText("UI: Editing...", Color.OrangeRed);
        }
        public static void ExitEditMode()
        {
            ModContent.GetInstance<UICustomizerSystem>().uiCustomizerState.saveButton.Active = false;
            ModContent.GetInstance<UICustomizerSystem>().uiCustomizerState.cancelButton.Active = false;
            EditModeActive = false;
            Main.NewText("UI: Saved!", Color.LightGreen);
        }

        // UI.
        public UserInterface userInterface;
        public UICustomizerState uiCustomizerState;

        public override void OnWorldLoad()
        {
            userInterface = new UserInterface();
            uiCustomizerState = new UICustomizerState();
            userInterface.SetState(uiCustomizerState);
        }


        public override void UpdateUI(GameTime gameTime)
        {
            userInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer(
                    "UICustomizer: UICustomizerSystem",
                    () => { userInterface?.Draw(Main.spriteBatch, new GameTime()); return true; },
                    InterfaceScaleType.UI)
                );
            }
        }
    }
}
