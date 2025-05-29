using System.Collections.Generic;
using UICustomizer.UI;

namespace UICustomizer.Common.Systems
{
    [Autoload(Side = ModSide.Client)]
    internal class UICustomizerSystem : ModSystem
    {
        // Handle edit mode state.
        public static bool EditModeActive { get; private set; } = true;
        public static void EnterEditMode()
        {
            EditModeActive = true;

            CombatText.NewText(Main.LocalPlayer.getRect(), Color.OrangeRed, "UI: Editing...");
        }
        public static void ExitEditMode()
        {
            EditModeActive = false;

            CombatText.NewText(Main.LocalPlayer.getRect(), Color.Green, "UI: Saved!");
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
