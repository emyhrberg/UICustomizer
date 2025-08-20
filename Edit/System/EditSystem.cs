using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.UI;
using UICustomizer.EditMode.UI;
using Terraria.ID;

namespace UICustomizer.EditMode.System
{
    [Autoload(Side = ModSide.Client)]
    public class EditSystem : ModSystem
    {
        // UI components
        public UserInterface ui;
        public EditState editState;
        public EditToggleButton icon;

        public bool Enabled; // whether the edit menu is visible

        public void Toggle()
        {
            Enabled = !Enabled;
            SoundEngine.PlaySound(SoundID.MenuTick);

            if (Enabled)
                Main.hidePlayerCraftingMenu = true;
        }

        public override void OnWorldLoad()
        {
            ui = new();
            editState = new();
            ui.SetState(editState);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            //if (ui.CurrentState != null)
            //Log.Info("state" + ui.CurrentState);
            //else
            //Log.Info("null");
            //ui.SetState(editState);

            if (ui?.CurrentState == editState)
                ui?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            // Main overlay
            int index = layers.FindIndex(l => l.Name == "Vanilla: Mouse Text");
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer(
                    "UI Editor: Edit System",
                    () =>
                    {
                        if (ui?.CurrentState == editState)
                            ui?.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}