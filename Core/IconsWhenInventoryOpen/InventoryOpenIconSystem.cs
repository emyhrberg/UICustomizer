using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using UIEditor.Core.IngameEditor.Systems;

namespace UIEditor.Core.IconsWhenInventoryOpen;

[Autoload(Side = ModSide.Client)]
public class InventoryOpenIconSystem : ModSystem
{
    // UI components
    public UserInterface ui;
    public InventoryOpenIconState state;

    public override void OnWorldLoad()
    {
        ui = new UserInterface();
        state = new InventoryOpenIconState();
        ui.SetState(null);
    }

    public override void UpdateUI(GameTime gameTime)
    {
        // Show icons only when inventory is open
        if (Main.playerInventory)
        {
            if (ui.CurrentState == null)
                ui.SetState(state);

            ui.Update(gameTime);
        }
        else
        {
            if (ui.CurrentState != null)
                ui.SetState(null);
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int mouseText = layers.FindIndex(l => l.Name == "Vanilla: Mouse Text");
        if (mouseText != -1)
        {
            layers.Insert(mouseText, new LegacyGameInterfaceLayer(
                "UIEditor: Draw Icons When Inventory Is Open",
                () =>
                {
                    if (ui.CurrentState != null)
                        ui.Draw(Main.spriteBatch, new GameTime());

                    return true;
                },
                InterfaceScaleType.UI));
        }
    }
}
