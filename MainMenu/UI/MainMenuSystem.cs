using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using UICustomizer.MainMenu.UI;

[Autoload(Side = ModSide.Client)]
internal sealed class MainMenuSystem : ModSystem
{
    public UserInterface ui;
    public MainMenuState mainState;
    public MainMenuEyeState eyeState;

    public override void PostSetupContent()
    {

        ui = new UserInterface();

        mainState = new();
        eyeState = new();

        // Create eye button for both states
        mainState.eyeButton = new(Ass.Inventory_Tick_On, () => ui.SetState(eyeState));
        mainState.Append(mainState.eyeButton);
        eyeState.eyeButton = new(Ass.Inventory_Tick_Off, () => ui.SetState(mainState));
        eyeState.Append(eyeState.eyeButton);

        if (Conf.C is not null && Conf.C.EditMainMenu)
        {
            ui.SetState(mainState);
        }

        On_Main.DrawVersionNumber += DrawMenuUI;
        On_Main.UpdateUIStates += PostUpdateUIStates;
    }

    private void DrawMenuUI(On_Main.orig_DrawVersionNumber orig, Color menuColor, float upBump)
    {
        orig(menuColor, upBump);

        if (Conf.C != null && !Conf.C.EditMainMenu) return;

        if (Main.gameMenu && Main.menuMode == 0 && ui?.CurrentState != null)
            ui.Draw(Main.spriteBatch, new GameTime());
    }

    private void PostUpdateUIStates(On_Main.orig_UpdateUIStates orig, GameTime gameTime)
    {
        orig(gameTime);

        if (Conf.C != null && !Conf.C.EditMainMenu)
        {
            ui.SetState(null);
            return;
        }

        if (Main.gameMenu && Main.menuMode == 0)
        {
            if (ui.CurrentState == null)
                ui.SetState(mainState);

            ui.Update(gameTime);
        }
        else if (ui.CurrentState != null)
        {
            ui.SetState(null);
        }

    }

    public override void Unload()
    {
        On_Main.DrawVersionNumber -= DrawMenuUI;
        On_Main.UpdateUIStates -= PostUpdateUIStates;
        ui = null;
        mainState = null;
        eyeState = null;
    }
}
