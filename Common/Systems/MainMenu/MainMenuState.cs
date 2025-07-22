using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using UICustomizer.Common.Configs;
using UICustomizer.Common.Systems.Hooks.MainMenu;
using UICustomizer.UI.MainMenuElements;
using UICustomizer.UI.MainMenuElements.TagElements;

namespace UICustomizer.Common.Systems.MainMenu;

public sealed class MainMenuState : UIState
{
    // Parent Elements
    private UIText headerText;
    private ExpandablePanel panel;  
    private UIList list;      

    public enum TextColorType
    {
        Fill,
        Outline,
        Hover
    }

    // Outline Text Color Section
    private BoxSection outlineTextColorSection;
    private ResetTag outlineReset;
    private HueSlider outlineHue;  
    private CopyTag outlineCopy;
    private PasteTag outlinePaste;
    private RandomizeTag outlineRand;
    private PanelTag outlinePanelTag;
    private Color _lastOutlineColor = MainMenuOutlineTextColorHook.Color;

    // Fill Text Color Section
    private BoxSection fillTextColorSection;
    private ResetTag fillReset;
    private HueSlider fillHue;
    private CopyTag fillCopy;
    private PasteTag fillPaste;
    private RandomizeTag fillRand;
    private PanelTag fillPanelTag;
    private Color _lastFillColor = MainMenuFillTextColorHook.Color;

    // Hover Text Color Section
    private BoxSection hoverTextColorSection;
    private ResetTag hoverReset;
    private HueSlider hoverHue;
    private CopyTag hoverCopy;
    private PasteTag hoverPaste;
    private RandomizeTag hoverRand;
    private PanelTag hoverPanelTag;
    private Color _lastHoverColor = MainMenuHoverTextColorHook.Color;

    // Time Section
    private BoxSection timeSection;
    private TimeSlider timeSlider;

    // Draw section
    private BoxSection drawSection;
    
    public MainMenuState()
    {
        // Null checks
        if (Conf.C is null || !Conf.C.EditMainMenu)
            return;

        panel = new();
        Append(panel);

        headerText = new("UI Editor", 1.15f) { HAlign = 0.5f, Top = { Pixels = 6f } };
        panel.Append(headerText);

        BuildList();

        panel.OnExpanded += () => { 
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
            MinHeight = { Pixels = 700 }, // total height for panel expanded is hardcoded for some reason
            Top = { Pixels = 30f },
            ListPadding = 4f
        };
        panel.VisibleWhenExpanded.Add(list);

        BuildFillTextColorSection(TextColorType.Fill);
        BuildOutlineTextColorSection(TextColorType.Outline);
        BuildHoverTextColorSection(TextColorType.Hover);
        BuildTimeSection();
        BuildDrawSection();
    }

    private void BuildFillTextColorSection(TextColorType type)
    {
        fillTextColorSection = new();

        UIElement topRow = new() { Width = { Percent = 1f }, Height = { Pixels = 24f } };
        UIText header = new("Fill Text Color") { HAlign = 0.5f, VAlign = 0.5f, Top = { Pixels = 4f } };
        fillReset = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/HairStyle_Arrow"), type)
        {
            HAlign = 1f,
            Top = { Pixels = -3f },
            Left = { Pixels = -3f }
        };

        topRow.Append(header);
        topRow.Append(fillReset);
        fillTextColorSection.Append(topRow);

        fillHue = new(type);
        fillTextColorSection.Append(fillHue);

        fillCopy = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Copy"), type);
        fillTextColorSection.Append(fillCopy);

        fillPaste = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Paste"), type);
        fillTextColorSection.Append(fillPaste);

        fillRand = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Randomize"), type);
        fillTextColorSection.Append(fillRand);

        fillPanelTag = new();
        fillTextColorSection.Append(fillPanelTag);

        list.Add(fillTextColorSection);
    }

    private void BuildOutlineTextColorSection(TextColorType type)
    {
        outlineTextColorSection = new();

        UIElement topRow = new() { Width = { Percent = 1f }, Height = { Pixels = 24f } };
        UIText header = new("Outline Text Color") { HAlign = 0.5f, VAlign = 0.5f, Top = { Pixels = 4f } };
        outlineReset = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/HairStyle_Arrow"), type)
        {
            HAlign = 1f,
            Top = { Pixels = -3f },
            Left = { Pixels = -3f }
        };

        topRow.Append(header);
        topRow.Append(outlineReset);
        outlineTextColorSection.Append(topRow);

        outlineHue = new(type);
        outlineTextColorSection.Append(outlineHue);

        outlineCopy = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Copy"), type);
        outlineTextColorSection.Append(outlineCopy);

        outlinePaste = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Paste"), type);
        outlineTextColorSection.Append(outlinePaste);

        outlineRand = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Randomize"), type);
        outlineTextColorSection.Append(outlineRand);

        outlinePanelTag = new();
        outlineTextColorSection.Append(outlinePanelTag);

        list.Add(outlineTextColorSection);
    }

    private void BuildHoverTextColorSection(TextColorType type)
    {
        hoverTextColorSection = new();

        UIElement topRow = new() { Width = { Percent = 1f }, Height = { Pixels = 24f } };
        UIText header = new("Hover Text Color") { HAlign = 0.5f, VAlign = 0.5f, Top = { Pixels = 4f } };
        hoverReset = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/HairStyle_Arrow"), type)
        {
            HAlign = 1f,
            Top = { Pixels = -3f },
            Left = { Pixels = -3f }
        };

        topRow.Append(header);
        topRow.Append(hoverReset);
        hoverTextColorSection.Append(topRow);

        hoverHue = new(type);
        hoverTextColorSection.Append(hoverHue);

        hoverCopy = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Copy"), type);
        hoverTextColorSection.Append(hoverCopy);

        hoverPaste = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Paste"), type);
        hoverTextColorSection.Append(hoverPaste);

        hoverRand = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Randomize"), type);
        hoverTextColorSection.Append(hoverRand);

        hoverPanelTag = new();
        hoverTextColorSection.Append(hoverPanelTag);

        list.Add(hoverTextColorSection);
    }

    private void BuildTimeSection()
    {
        timeSection = new();
        list.Add(timeSection);

        UIText timeHeader = new("Time") { HAlign = 0.5f };
        timeSection.Append(timeHeader);

        timeSlider = new();
        timeSection.Append(timeSlider);

        UIText timeText = new(WorldTimeHelper.GetFormattedTime()) { Top = { Pixels = 50 }, HAlign = 0.5f };
        timeSection.Append(timeText);

        UIText pauseToggle = new("Pause: Off") { Top = { Pixels = 50 + 24 }, HAlign = 0.5f };
        timeSection.Append(pauseToggle);

        UIText dayRate = new("Main.time: ") { Top = { Pixels = 74 + 24 }, HAlign = 0.5f };
        timeSection.Append(dayRate);

        pauseToggle.OnLeftMouseDown += (_, __) =>
        {
            MainMenuPauseSystem.TimeIsPausedBySlider = !MainMenuPauseSystem.TimeIsPausedBySlider;
            refresh();
        };

        void refresh()
        {
            timeText.SetText(WorldTimeHelper.GetFormattedTime());
            timeSlider.Ratio = WorldTimeHelper.GetRatioFromTime();
            pauseToggle.SetText($"Pause: {(MainMenuPauseSystem.TimeIsPausedBySlider ? "On" : "Off")}");
            dayRate.SetText($"Main.time: {(int)Main.time}");
        }

        timeSection.OnUpdate += _ => refresh();
    }

    private void BuildDrawSection()
    {
        drawSection = new();
        list.Add(drawSection);

        UIText drawHeader = new("Draw") { HAlign = 0.5f };
        drawSection.Append(drawHeader);

        UIText logoToggle = new("Logo: Off") { Top = { Pixels = 30 }, HAlign = 0.5f };
        drawSection.Append(logoToggle);

        logoToggle.OnLeftMouseDown += (_, __) =>
        {
            // TODO impl logo toggle 
            MainMenuPauseSystem.TimeIsPausedBySlider = !MainMenuPauseSystem.TimeIsPausedBySlider;
            logoToggle.SetText($"Logo: {(MainMenuPauseSystem.TimeIsPausedBySlider ? "On" : "Off")}");
        };
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);

        DrawHoverTooltips(sb);
    }

    private void DrawHoverTooltips(SpriteBatch sb)
    {
        UIElement[] hoverables =
        [
        fillCopy, fillPaste, fillRand, fillReset,
        outlineCopy, outlinePaste, outlineRand, outlineReset,
        hoverCopy, hoverPaste, hoverRand, hoverReset
        ];

        foreach (UIElement element in hoverables)
        {
            if (!element.IsMouseHovering)
                continue;

            string text = element switch
            {
                CopyTag => "Copy color",
                PasteTag => "Paste color",
                RandomizeTag => "Randomize color",
                ResetTag => "Reset",
                _ => null
            };

            if (text != null)
            {
                DrawHelper.DrawTextAtMouse(sb, text);
                break; // only draw tooltip for the first hovered
            }
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        UpdateOutlineTextColor();
        UpdateFillTextColor();
        UpdateHoverTextColor();
    }

    private void UpdateOutlineTextColor()
    {
        Color currentOutline = MainMenuOutlineTextColorHook.Color;

        if (currentOutline != _lastOutlineColor)
        {
            _lastOutlineColor = currentOutline;

            // 1) move the hue slider knob  (HSV hue in [0,1])
            float h = Main.rgbToHsl(currentOutline).X;
            outlineHue.Ratio = h;

            // 2) update the little #RRGGBB label
            outlinePanelTag.tagText.SetText($"#{currentOutline.R:X2}{currentOutline.G:X2}{currentOutline.B:X2}");
        }
    }

    private void UpdateFillTextColor()
    {
        Color currentFill = MainMenuFillTextColorHook.Color;
        if (currentFill != _lastFillColor)
        {
            _lastFillColor = currentFill;

            float h = Main.rgbToHsl(currentFill).X;
            fillHue.Ratio = h;

            fillPanelTag.tagText.SetText($"#{currentFill.R:X2}{currentFill.G:X2}{currentFill.B:X2}");
        }
    }

    private void UpdateHoverTextColor()
    {
        Color currentHover = MainMenuHoverTextColorHook.Color;
        if (currentHover != _lastHoverColor)
        {
            _lastHoverColor = currentHover;

            float h = Main.rgbToHsl(currentHover).X;
            hoverHue.Ratio = h;

            hoverPanelTag.tagText.SetText($"#{currentHover.R:X2}{currentHover.G:X2}{currentHover.B:X2}");
        }
    }
}
