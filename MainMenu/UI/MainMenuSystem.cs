using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using UICustomizer.Common.Systems.MainMenu;

namespace UICustomizer.MainMenu;

[Autoload(Side = ModSide.Client)]
internal sealed class MainMenuSystem : ModSystem
{
    public UserInterface ui;
    public MainMenuState state;
    public MainMenuEyeState eyeState;

    public override void PostSetupContent()
    {
        if (Conf.C is null || !Conf.C.EditMainMenu) return;

        // Initialize the user interface
        ui = new UserInterface();

        // Setup main menu state
        state = new MainMenuState();
        eyeState = new MainMenuEyeState();

        state.eyeToggle.OnLeftClick += (_, _) => ui.SetState(eyeState);
        eyeState.eyeToggle.OnLeftClick += (_, _) => ui.SetState(state);

        // Set the initial state to the main menu state
        ui.SetState(state);

        // Hook into the main menu drawing and updating
        On_Main.DrawVersionNumber += DrawMenuUI;
        On_Main.UpdateUIStates += PostUpdateUIStates;
    }

    private void DrawMenuUI(On_Main.orig_DrawVersionNumber orig, Color menuColor, float upBump)
    {
        orig(menuColor, upBump);
        if (Main.gameMenu && Main.menuMode == 0 && ui?.CurrentState != null)
        {
            //Main.spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);
            ui.Draw(Main.spriteBatch, new GameTime());
            //Main.spriteBatch.End();
        }
    }

    private void PostUpdateUIStates(On_Main.orig_UpdateUIStates orig, GameTime gameTime)
    {
        ui.SetState(null);

        if (Main.gameMenu && Main.menuMode == 0)
        {
            if (ui.CurrentState == null)
                ui.SetState(state);

            ui.Update(gameTime);
        }
        else if (ui.CurrentState != null)
        {
            ui.SetState(null);
        }

        orig(gameTime);
    }

    public override void Unload()
    {
        On_Main.DrawVersionNumber -= DrawMenuUI;
        On_Main.UpdateUIStates -= PostUpdateUIStates;
        ui = null;
        state = null;
    }
}