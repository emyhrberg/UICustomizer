using System;
using Humanizer;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;
using UICustomizer.Common.Configs;
using UICustomizer.Common.Systems.Hooks.MainMenu;
using UICustomizer.UI.MainMenuElements;
using UICustomizer.UI.MainMenuElements.TagElements;

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

    public enum TextColorType
    {
        Fill,
        Outline,
        Hover
    }

    // Outline
    private HueSlider outlineHue;
    private PanelTag outlinePanelTag;
    private Color _lastOutlineColor = MainMenuOutlineTextColorHook.Color;

    // Fill
    private HueSlider fillHue;
    private PanelTag fillPanelTag;
    private Color _lastFillColor = MainMenuTextColorHook.NormalColor;

    // Hover
    private HueSlider hoverHue;
    private PanelTag hoverPanelTag;
    private Color _lastHoverColor = MainMenuTextColorHook.HoverColor;

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
            MinHeight = { Pixels = 700 }, // total height for panel expanded is hardcoded for some reason
            Top = { Pixels = 30f },
            ListPadding = 4f,
            ManualSortMethod = _ => { }
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
        BoxSection fillTextColorSection = new ();

        UIText header = new("Fill") { HAlign = 0.5f, Top = { Pixels = 4f } };
        fillTextColorSection.Append(header);

        ResetButton fillReset = new(Ass.Reset, type);
        fillTextColorSection.Append(fillReset);

        fillHue = new(type);
        fillHue.Ratio = Main.rgbToHsl(MainMenuTextColorHook.NormalColor).X;
        fillTextColorSection.Append(fillHue);

        CopyTag fillCopy = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Copy"), type);
        fillTextColorSection.Append(fillCopy);

        PasteTag fillPaste = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Paste"), type);
        fillTextColorSection.Append(fillPaste);

        RandomizeTag fillRand = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Randomize"), type);
        fillTextColorSection.Append(fillRand);

        fillPanelTag = new();
        fillPanelTag.tagText.SetText($"#{MainMenuTextColorHook.NormalColor.R:X2}{MainMenuTextColorHook.NormalColor.G:X2}{MainMenuTextColorHook.NormalColor.B:X2}");
        fillTextColorSection.Append(fillPanelTag);

        list.Add(fillTextColorSection);
    }

    private void BuildOutlineTextColorSection(TextColorType type)
    {
        BoxSection outlineTextColorSection = new ();

        UIText header = new("Outline") { HAlign = 0.5f, Top = { Pixels = 4f } };
        outlineTextColorSection.Append(header);

        ResetButton outlineReset = new (Ass.Reset, type);
        outlineTextColorSection.Append(outlineReset);

        outlineHue = new(type);
        outlineHue.Ratio = Main.rgbToHsl(MainMenuOutlineTextColorHook.Color).X;
        outlineTextColorSection.Append(outlineHue);

        CopyTag outlineCopy = new (Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Copy"), type);
        outlineTextColorSection.Append(outlineCopy);

        PasteTag outlinePaste = new (Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Paste"), type);
        outlineTextColorSection.Append(outlinePaste);

        RandomizeTag outlineRand = new (Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Randomize"), type);
        outlineTextColorSection.Append(outlineRand);

        outlinePanelTag = new();
        outlinePanelTag.tagText.SetText($"#{MainMenuOutlineTextColorHook.Color.R:X2}{MainMenuOutlineTextColorHook.Color.G:X2}{MainMenuOutlineTextColorHook.Color.B:X2}");
        outlineTextColorSection.Append(outlinePanelTag);

        list.Add(outlineTextColorSection);
    }

    private void BuildHoverTextColorSection(TextColorType type)
    {
        BoxSection hoverTextColorSection = new();

        UIText header = new("Hover") { HAlign = 0.5f, Top = { Pixels = 4f } };
        hoverTextColorSection.Append(header);
        
        ResetButton hoverReset = new(Ass.Reset, type);
        hoverTextColorSection.Append(hoverReset);

        hoverHue = new(type);
        hoverHue.Ratio = Main.rgbToHsl(MainMenuTextColorHook.HoverColor).X;
        hoverTextColorSection.Append(hoverHue);

        CopyTag hoverCopy = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Copy"), type);
        hoverTextColorSection.Append(hoverCopy);

        PasteTag hoverPaste = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Paste"), type);
        hoverTextColorSection.Append(hoverPaste);

        RandomizeTag hoverRand = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Randomize"), type);
        hoverTextColorSection.Append(hoverRand);

        hoverPanelTag = new();
        hoverPanelTag.tagText.SetText($"#{MainMenuTextColorHook.HoverColor.R:X2}{MainMenuTextColorHook.HoverColor.G:X2}{MainMenuTextColorHook.HoverColor.B:X2}");
        hoverTextColorSection.Append(hoverPanelTag);


        list.Add(hoverTextColorSection);
    }

    private void BuildTimeSection()
    {
        BoxSection timeSection = new();

        UIText timeHeader = new("Time") { HAlign = 0.5f, Top = { Pixels = 6 } };
        timeSection.Append(timeHeader);

        PlayPause playPause = new();
        playPause.OnLeftMouseDown += (_, __) => MainMenuPauseSystem.IsPaused = !MainMenuPauseSystem.IsPaused;
        timeSection.Append(playPause);

        TimeSlider timeSlider = new () { Top = { Pixels = 35 } };
        timeSection.Append(timeSlider);

        UIText speedHeader = new("Speed") { HAlign = 0.5f, Top = { Pixels = 66 } };
        timeSection.Append(speedHeader);

        var timeSpeedSlider = new TimeSpeedSlider();
        timeSection.Append(timeSpeedSlider);

        ResetButton resetSpeed = new(Ass.Reset) { Top = { Pixels = 60 } };
        resetSpeed.OnLeftMouseDown += (_, __) =>
        {
            timeSpeedSlider.Ratio = ColorHelper.InverseLerp(-20f, 20f, 1f);
            TimeSpeedHook.MenuTimeSpeed = 1;
        };
        timeSection.Append(resetSpeed);

        timeSection.OnUpdate += _ =>
        {
            speedHeader.SetText("Speed: " + (float)Math.Round(TimeSpeedHook.MenuTimeSpeed, 2));

            timeHeader.SetText("Time: " + WorldTimeHelper.GetFormattedTime());
            timeSlider.Ratio = WorldTimeHelper.GetRatioFromTime();
        };

        list.Add(timeSection);
    }
    
    private void BuildDrawSection()
    {
        BoxSection drawSection = new();
        list.Add(drawSection);

        UIText drawHeader = new("Draw") { HAlign = 0.5f, Top = { Pixels = 4 } };
        drawSection.Append(drawHeader);

        OnOffTextButton backgroundToggle = new("Draw Background: ") { Top = { Pixels = 90 } };
        backgroundToggle.OnLeftClick += (_, _) => SkipBackgroundDrawHook.IsDrawing = !SkipBackgroundDrawHook.IsDrawing;
        drawSection.Append(backgroundToggle);

        OnOffTextButton logoToggle = new("Draw Logo: ") { Top = { Pixels = 30 } };
        logoToggle.OnLeftClick += (_, _) => SkipLogoDrawHook.IsDrawing = !SkipLogoDrawHook.IsDrawing;
        drawSection.Append(logoToggle);

        OnOffTextButton socialMediaToggle = new("Draw Social Buttons: ") { Top = { Pixels = 50 } };
        socialMediaToggle.OnLeftClick += (_, _) => SkipSocialMediaButtonsHook.IsDrawing = !SkipSocialMediaButtonsHook.IsDrawing;
        drawSection.Append(socialMediaToggle);

        OnOffTextButton versionNumberToggle = new("Draw Version Number: ") { Top = { Pixels = 70 } };
        versionNumberToggle.OnLeftClick += (_, _) => SkipVersionNumberDrawHook.IsDrawing = !SkipVersionNumberDrawHook.IsDrawing;
        drawSection.Append(versionNumberToggle);
    }

    public override void Draw(SpriteBatch sb)
    {
        if (!Conf.C.ShowMainMenu) return;

        base.Draw(sb);
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

            float h = Main.rgbToHsl(currentOutline).X;
            outlineHue.Ratio = h;

            outlinePanelTag.tagText.SetText($"#{currentOutline.R:X2}{currentOutline.G:X2}{currentOutline.B:X2}");
        }
    }

    private void UpdateFillTextColor()
    {
        Color currentFill = MainMenuTextColorHook.NormalColor;
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
        Color currentHover = MainMenuTextColorHook.HoverColor;
        if (currentHover != _lastHoverColor)
        {
            _lastHoverColor = currentHover;

            float h = Main.rgbToHsl(currentHover).X;
            hoverHue.Ratio = h;

            hoverPanelTag.tagText.SetText($"#{currentHover.R:X2}{currentHover.G:X2}{currentHover.B:X2}");
        }
    }
}
