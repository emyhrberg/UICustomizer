using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;

namespace UICustomizer.MainMenu.UI;

public sealed class MainMenuEyeState : UIState
{
    public EyeButton eyeToggle;

    public MainMenuEyeState()
    {
        // Null checks
        if (Conf.C is null || !Conf.C.EditMainMenu)
            return;

        eyeToggle = new EyeButton(Ass.Inventory_Tick_Off);
        eyeToggle.isOn = false;
        Append(eyeToggle);
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }
}
