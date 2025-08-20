using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;
using UICustomizer.MainMenu.UI.Sections;

namespace UICustomizer.MainMenu.UI;

public sealed class MainMenuState : UIState
{
    // Parent Elements
    private ExpandablePanel panel;
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
            MinHeight = { Pixels = 795 }, // total height for colorPanel expanded is hardcoded for some reason
            Top = { Pixels = 30 },
            ListPadding = 0f, // AFFECTS ALL SECTIONS!
            ManualSortMethod = _ => { }
        };
        panel.VisibleWhenExpanded.Add(list);

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
