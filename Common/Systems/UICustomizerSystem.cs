using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using UICustomizer.UI;

namespace UICustomizer.Common.Systems
{
    [Autoload(Side = ModSide.Client)]
    internal class UICustomizerSystem : ModSystem
    {
        // Handle edit mode state.
        public static bool EditModeActive { get; private set; } = true;
        public static void EnterEditMode() => EditModeActive = true;
        public static void ExitEditMode() => EditModeActive = false;

        // UI components
        public UserInterface userInterface;
        public UICustomizerState state;

        public override void OnWorldLoad()
        {
            userInterface = new UserInterface();
            state = new UICustomizerState();
            userInterface.SetState(state);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            userInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseText = layers.FindIndex(l => l.Name == "Vanilla: Mouse Text");
            if (mouseText != -1)
            {
                layers.Insert(mouseText, new LegacyGameInterfaceLayer(
                    "UICustomizer: UI",
                    () => { userInterface?.Draw(Main.spriteBatch, new GameTime()); return true; },
                    InterfaceScaleType.UI));
            }
        }
    }
}
