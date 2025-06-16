using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using UICustomizer.Common.States;

namespace UICustomizer.Common.Systems
{
    internal class EditModeSystem : ModSystem
    {
        public static bool IsActive { get; private set; }

        public static void SetActive(bool active)
        {
            IsActive = active;
        }

        public static void ToggleActive()
        {
            SetActive(!IsActive);
        }

        // UI components
        public UserInterface userInterface;
        public EditModeState state;

        public override void OnModLoad()
        {
            base.OnModLoad();
        }

        public override void OnWorldLoad()
        {
            base.OnWorldLoad();
            userInterface = new();
            state = new();
            userInterface.SetState(state);

            //SetActive(true); // DEBUG MODE ON LAUNCH
        }

        public override void UpdateUI(GameTime gameTime)
        {
            userInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            // Main overlay
            int mouseText = layers.FindIndex(l => l.Name == "Vanilla: Mouse Text");
            if (mouseText != -1)
            {
                layers.Insert(mouseText, new LegacyGameInterfaceLayer(
                    name: "UICustomizer: EditModeSystem",
                    drawMethod: () =>
                    {
                        userInterface?.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    scaleType: InterfaceScaleType.UI));
            }
        }
    }
}
