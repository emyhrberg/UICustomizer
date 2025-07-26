using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using UICustomizer.Common.Configs;
using static Terraria.GameContent.PlayerEyeHelper;

namespace UICustomizer.Common.Systems.MainMenu;

[Autoload(Side = ModSide.Client)]
internal sealed class MainMenuSystem : ModSystem
{
    private UserInterface ui;
    public MainMenuState state;
    public MainMenuEyeState eyeState;

    public override void PostSetupContent()
    {
        if (Conf.C is null || !Conf.C.EditMainMenu) return;

        ui = new UserInterface();
        state = new MainMenuState();
        ui.SetState(state);

        On_Main.DrawVersionNumber += DrawMenuUI;
        On_Main.UpdateUIStates += PostUpdateUIStates;

        // Setup eye toggle
        eyeState = new MainMenuEyeState();
        state.eyeToggle.OnLeftClick += (_, _) => { ui.SetState(eyeState); };
        eyeState.eyeToggle.OnLeftClick += (_, _) => ui.SetState(state);
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