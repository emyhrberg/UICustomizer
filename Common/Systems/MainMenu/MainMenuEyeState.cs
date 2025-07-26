using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;
using UICustomizer.Common.Configs;
using UICustomizer.Common.Systems.Hooks.MainMenu;
using UICustomizer.UI;
using UICustomizer.UI.MainMenuElements;
using UICustomizer.UI.MainMenuElements.Sections;

namespace UICustomizer.Common.Systems.MainMenu;

public sealed class MainMenuEyeState : UIState
{
    public EyeButton eyeToggle;

    public MainMenuEyeState()
    {
        // Null checks
        if (Conf.C is null || !Conf.C.EditMainMenu)
            return;

        eyeToggle = new EyeButton(Ass.Inventory_Tick_On);
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
