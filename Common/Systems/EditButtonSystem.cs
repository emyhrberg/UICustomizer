using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;
using UICustomizer.Common.States;

namespace UICustomizer.Common.Systems
{
    internal class EditButtonSystem : ModSystem
    {
        public UserInterface ui;
        public EditButtonState state;

        public override void Load()
        {
            ui = new();
            state = new();
            ui.SetState(state);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            ui?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int idx = layers.FindIndex(l => l.Name == "Vanilla: Mouse Text");
            if (idx == -1) return;

            layers.Insert(idx, new LegacyGameInterfaceLayer(
                "UICustomizer: Edit Button",
                () =>
                {
                    ui?.Draw(Main.spriteBatch, Main._drawInterfaceGameTime);
                    return true;
                },
                InterfaceScaleType.UI
            ));
        }
    }
}