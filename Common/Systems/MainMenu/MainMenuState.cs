using DragonLens.Content.Tools.Gameplay;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.OS;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;
using Terraria.UI;
using UICustomizer.Common.Configs;
using UICustomizer.Common.Systems.Hooks.MainMenu;
using UICustomizer.UI;

namespace UICustomizer.Common.Systems.MainMenu;

internal sealed class MainMenuState : UIState
{
    // Elements
    private UIList mainMenuList;   // outer list that lives directly on the menu
    private UIExpandablePanel panel;        // collapsible panel (header + body)
    private NestedUIList innerList;      // scrollable list visible only when expanded
    private UIScrollbar scrollbar;      // scrollbar for the inner list

    // Fields
    private Vector3 MainMenuFillColor;
    private Vector3 MainMenuOutlineColor;
    private Vector3 MainMenuHoverColor;

    public MainMenuState()
    {
        // Null checks
        if (Conf.C is null || !Conf.C.EditMainMenu)
            return;

        // Set up UIList
        mainMenuList = new UIList
        {
            Width = { Pixels = 250 },
            Height = StyleDimension.Fill,
            Left = { Pixels = Main.screenWidth - 260 },
            Top = { Pixels = 15f },
            ListPadding = 0f,
            ManualSortMethod = _ => { } // keep insertion order
        };
        Append(mainMenuList);

        panel = new UIExpandablePanel();
        panel.Collapse(); // start collapsed
        mainMenuList.Add(panel);

        var header = new UIText("UI Editor");
        header.HAlign = 0.5f;
        header.Top.Set(4f, 0f);
        panel.Append(header);
        //BuildInnerList(); // guh first click no work but maybe this breaks
        panel.OnExpanded += () => { BuildInnerList(); panel.Recalculate(); };
        Append(mainMenuList);
    }

    private void BuildInnerList()
    {
        if (innerList != null) return;

        innerList = new NestedUIList
        {
            Width = { Pixels = -22f, Percent = 1f },
            MinHeight = { Pixels = 330 },   // enough to show both sections
            //Height = { Pixels = Main.screenHeight - 100 },
            Top = { Pixels = 30f },
            ListPadding = 4f
        };
        panel.VisibleWhenExpanded.Add(innerList);

        BuildTextColorSection();
        BuildTimeSection();
    }

    private void BuildTextColorSection()
    {
        var box = new UIPanel
        {
            Width = { Pixels = 0, Percent = 1f },
            Height = { Pixels = 130 },
            HAlign = 0.5f,
            PaddingTop = 4f
        };
        var textColorHeader = new UIText("Text Color") { HAlign = 0.5f, Top = { Pixels = 2 } };
        box.Append(textColorHeader);
        innerList.Add(box);

        var hue = new ZenSlider();
        hue.InnerTexture = Ass.SliderHueGradient;
        box.Append(hue);

        var tagPanel = new UIPanel
        {
            Width = { Pixels = 80 },
            Height = { Pixels = 26 },
            HAlign = 1f,
            VAlign = 1f,
            Left = { Pixels = 5 }
        };
        box.Append(tagPanel);

        void apply(Color c)
        {
            MainMenuTextColorHook.MainMenuTextColor = c;
            tagPanel.BackgroundColor = c;
        }

        hue.OnDrag += v => apply(UICharacterCreation.ScaledHslToRgb(v, 1f, 0.5f));
        hue.OnValueAppliedOnMouseUp += v => apply(UICharacterCreation.ScaledHslToRgb(v, 1f, 0.5f));

        UIColoredImageButton make(string name, float left) =>
            new(Main.Assets.Request<Texture2D>($"Images/UI/CharCreation/{name}"), true)
            {
                Left = { Pixels = left },
                VAlign = 1f
            };

        var tagText = new UIText("FFFFFF") { HAlign = .5f, VAlign = .5f };
        tagPanel.Append(tagText);
        box.Append(tagPanel);

        var copy = make("Copy", 0);
        copy.OnLeftMouseDown += (_, __) =>
        {
            var cb = Platform.Get<IClipboard>();
            cb.Value = tagText.Text;
        };
        box.Append(copy);

        var paste = make("Paste", 40);
        paste.OnLeftMouseDown += (_, __) =>
        {
            var cb = Platform.Get<IClipboard>();
            var hex = cb.Value?.Trim().TrimStart('#');
            if (!string.IsNullOrEmpty(hex) && hex.Length == 6 &&
                int.TryParse(hex, System.Globalization.NumberStyles.HexNumber,
                             System.Globalization.CultureInfo.InvariantCulture, out var rgb))
                apply(new Color((rgb >> 16) & 255, (rgb >> 8) & 255, rgb & 255));
        };
        box.Append(paste);

        var rand = make("Randomize", 80);
        rand.OnLeftMouseDown += (_, __) =>
        {
            var c = new Color(Main.rand.Next(256), Main.rand.Next(256), Main.rand.Next(256));
            apply(c);
        };
        box.Append(rand);
    }

    private void BuildTimeSection()
    {
        var box = new UIPanel
        {
            Width = { Pixels = 0, Percent = 1f },
            Height = { Pixels = 104 },
            HAlign = 0.5f,
            PaddingTop = 3f
        };
        innerList.Add(box);

        var header = new UIText("Time") { HAlign = 0.5f };
        box.Append(header);

        var slider = new ZenSlider
        {
            Width = { Percent = 1f, Pixels = -10 },
            Top = { Pixels = 24 }
        };
        slider.Ratio = WorldTimeHelper.GetRatioFromTime();
        box.Append(slider);

        var timeText = new UIText(WorldTimeHelper.GetFormattedTime()) { Top = { Pixels = 50 }, HAlign = 0.5f };
        box.Append(timeText);

        var pauseToggle = new UIText("Pause: Off") { Top = { Pixels = 74 }, HAlign = 0.5f };
        box.Append(pauseToggle);

        void refresh()
        {
            timeText.SetText(WorldTimeHelper.GetFormattedTime());
            slider.Ratio = WorldTimeHelper.GetRatioFromTime();
            pauseToggle.SetText($"Pause: {(MainMenuPauseSystem.TimeIsPausedBySlider ? "On" : "Off")}");
        }

        slider.OnDrag += v =>
        {
            WorldTimeHelper.SetTime(v);
            refresh();
        };
        slider.OnValueAppliedOnMouseUp += v =>
        {
            WorldTimeHelper.SetTime(v);
            refresh();
        };

        pauseToggle.OnLeftMouseDown += (_, __) =>
        {
            MainMenuPauseSystem.TimeIsPausedBySlider = !MainMenuPauseSystem.TimeIsPausedBySlider;
            refresh();
        };

        box.OnUpdate += _ => refresh();
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
