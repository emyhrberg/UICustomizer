using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;
using UICustomizer.MainMenu.UI;
using UICustomizer.MainMenu.UI.Sections;

namespace UICustomizer.MainMenu.System;

public sealed class MainMenuState : UIState
{
    // Parent Elements
    private UIPanel panel;
    private UIList list;

    // Header Elements
    private UIText headerText;
    private ConfigButton configButton;
    public EyeButton eyeButton;

    public MainMenuState()
    {
        // Null checks
        if (Conf.C is null || !Conf.C.EditMainMenu)
            return;

        panel = new();
        panel.Width.Set(320, 0);
        panel.Height.Set(795+30, 0);
        panel.OverflowHidden = false;
        panel.BackgroundColor = ColorHelper.DarkBluePanel * 0.5f;
        panel.SetPadding(0);
        panel.HAlign = 1f;
        panel.Top.Set(3, 0);
        panel.Left.Set(-3, 0);
        Append(panel);

        headerText = new("UI Editor", 1.15f) { HAlign = 0.5f, Top = { Pixels = 6f } };
        panel.Append(headerText);

        // Eye button created in parent (MainMenuSystem)
        //eyeButton = new EyeButton();
        //panel.Append(eyeButton);

        configButton = new ConfigButton(UICommon.ButtonModConfigTexture);
        panel.Append(configButton);

        // Build list on expanding
        BuildList();
    }

    private void BuildList()
    {
        if (list != null) return;

        list = new UIList
        {
            Width = { Pixels = -22f, Percent = 1f },
            MinHeight = { Pixels = 795 }, // total height for colorPanel expanded is hardcoded for some reason
            Top = { Pixels = 30 },
            ListPadding = 0f, // AFFECTS ALL SECTIONS!
            ManualSortMethod = _ => { }
        };

        // Add all sections
        TextColorSection textColorSection = new();
        TimeSection timeSection = new();
        BackgroundSection backgroundSection = new();
        LogoSection logoSection = new();
        DrawSection drawSection = new();

        list.Add(textColorSection);
        list.Add(timeSection);
        list.Add(backgroundSection);
        list.Add(logoSection);
        list.Add(drawSection);
        panel.Append(list);
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
