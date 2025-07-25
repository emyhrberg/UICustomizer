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

public sealed class MainMenuState : UIState
{
    // Parent Elements
    private ExpandablePanel panel;
    private UIList list;

    // Header Elements
    private UIText headerText;
    private ConfigButton configButton;
    public EyeButton eyeToggle;

    public MainMenuState()
    {
        // Null checks
        if (Conf.C is null || !Conf.C.EditMainMenu)
            return;

        panel = new();
        Append(panel);

        headerText = new("UI Editor", 1.15f) { HAlign = 0.5f, Top = { Pixels = 6f } };
        panel.Append(headerText);

        eyeToggle = new EyeButton(Ass.Inventory_Tick_On);
        panel.Append(eyeToggle);

        configButton = new ConfigButton(UICommon.ButtonModConfigTexture);
        panel.Append(configButton);

        // Build list on expanding
        BuildList();

        panel.OnExpanded += () =>
        {
            BuildList();
            panel.Recalculate();
        };
    }

    private void BuildList()
    {
        if (list != null) return;

        list = new UIList
        {
            Width = { Pixels = -22f, Percent = 1f },
            MinHeight = { Pixels = 750 }, // total height for colorPanel expanded is hardcoded for some reason
            Top = { Pixels = 30+6f },
            ListPadding = 12f, // AFFECTS ALL SECTIONS!
            ManualSortMethod = _ => { }
        };
        panel.VisibleWhenExpanded.Add(list);

        // Add all sections
        TextColorSection textColorSection = new();
        TimeSection timeSection = new();
        LogoSection logoSection = new();
        DrawSection drawSection = new();

        list.Add(textColorSection);
        list.Add(timeSection);
        list.Add(logoSection);
        list.Add(drawSection);
    }

    public override void Draw(SpriteBatch sb)
    {
        if (!Conf.C.ShowMainMenu) return;

        base.Draw(sb);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }
}
