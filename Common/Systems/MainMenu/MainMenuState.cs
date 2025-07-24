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
    public enum ColorTab { Fill, Outline, Hover }

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
            MinHeight = { Pixels = 750 }, // total height for panel expanded is hardcoded for some reason
            Top = { Pixels = 30f },
            ListPadding = 4f,
            ManualSortMethod = _ => { }
        };
        panel.VisibleWhenExpanded.Add(list);

        BuildColorSection();
        BuildTimeSection();
        BuildLogoSection();
        BuildDrawSection();
    }

    private void BuildColorSection()
    {
        BoxSectionHSLSlider section = new();
        list.Add(section);
    }
    private void BuildTimeSection()
    {
        BoxSection timeSection = new(130+64);

        // Time
        UIText timeHeader = new("Time: ") { HAlign = 0.5f, Top = { Pixels = 6 } };
        timeSection.Append(timeHeader);

        PlayPause playPause = new() { Left = { Pixels = 6 } };
        playPause.OnLeftMouseDown += (_, _) => MainMenuPauseSystem.IsPaused = !MainMenuPauseSystem.IsPaused;
        timeSection.Append(playPause);

        ZoeSlider timeSlider = new()
        {
            Top = { Pixels = 35 },
            Ratio = WorldTimeHelper.GetRatioFromTime()
        };
        timeSlider.OnDrag += WorldTimeHelper.SetTime;
        timeSection.Append(timeSlider);

        // Speed
        UIText speedHeader = new("Speed: ") { HAlign = 0.5f, Top = { Pixels = 66 } };
        timeSection.Append(speedHeader);

        ZoeSlider speedSlider = new()
        {
            Top = { Pixels = 94 },
            Ratio = ColorHelper.InverseLerp(0, 100, TimeSpeedHook.Speed)
        };
        speedSlider.OnDrag += value => TimeSpeedHook.Speed = MathHelper.Lerp(0, 100, value);
        timeSection.Append(speedSlider);

        ResetButton resetSpeed = new() { Top = { Pixels = 60 } };
        resetSpeed.OnLeftMouseDown += (_, _) => {
            TimeSpeedHook.Speed = 1;
            speedSlider.Ratio = ColorHelper.InverseLerp(0, 100, 1);
        };
        timeSection.Append(resetSpeed);

        // Parallax
        UIText parallaxHeader = new("Parallax: ") { HAlign = 0.5f, Top = { Pixels = 126 } };
        timeSection.Append(parallaxHeader);

        ZoeSlider parallaxSlider = new()
        {
            Top = { Pixels = 154 },
            Ratio = ColorHelper.InverseLerp(0f, 100f, ParallaxSpeedHook.Speed)
        };
        parallaxSlider.OnDrag += v => ParallaxSpeedHook.Speed = MathHelper.Lerp(0f, 100f, v);
        timeSection.Append(parallaxSlider);

        ResetButton resetParallax = new() { Top = { Pixels = 120 } };
        resetParallax.OnLeftMouseDown += (_, _) =>
        {
            ParallaxSpeedHook.Speed = 5f;
            parallaxSlider.Ratio = ColorHelper.InverseLerp(0f, 100f, ParallaxSpeedHook.Speed);
        };
        timeSection.Append(resetParallax);

        // Update
        timeSection.OnUpdate += _ =>
        {
            timeHeader.SetText("Time: " + WorldTimeHelper.GetFormattedTime());
            speedHeader.SetText("Speed: " + $"{TimeSpeedHook.Speed:F2}");
            parallaxHeader.SetText("Parallax: " + $"{ParallaxSpeedHook.Speed:F2}");
            timeSlider.Ratio = WorldTimeHelper.GetRatioFromTime(); // needed!
        };

        list.Add(timeSection);
    }
    private void BuildLogoSection()
    {
        BoxSection logoSection = new(130+30);

        // --- Logo Scale ---
        UIText logoScale = new("Logo Scale: ") { HAlign = 0.5f, Top = { Pixels = 6 } };
        logoSection.Append(logoScale);

        ZoeSlider scaleSlider = new()
        {
            Top = { Pixels = 35 },
            Ratio = ColorHelper.InverseLerp(0, 10, LogoHook.LogoScale) // = 1
        };
        scaleSlider.OnDrag += value => LogoHook.LogoScale = MathHelper.Lerp(0f, 10f, value);
        logoSection.Append(scaleSlider);

        ResetButton resetScale = new() { Top = { Pixels = 0 } };
        resetScale.OnLeftMouseDown += (_, __) => {
            LogoHook.LogoScale = 1f;
            scaleSlider.Ratio = ColorHelper.InverseLerp(0, 10, LogoHook.LogoScale);
        };
        logoSection.Append(resetScale);

        // --- Logo Rotation ---
        UIText logoRotation = new("Logo Rotation: ") { HAlign = 0.5f, Top = { Pixels = 66 } };
        logoSection.Append(logoRotation);

        ZoeSlider rotationSlider = new() { Top = { Pixels = 94 } };
        rotationSlider.OnDrag += value => LogoHook.LogoRotation = MathHelper.Lerp(0f, 6f, value);
        logoSection.Append(rotationSlider);

        ResetButton resetRotation = new() { Top = { Pixels = 60 } };
        resetRotation.OnLeftMouseDown += (_, _) =>
        {
            LogoHook.LogoRotation = 0;
            rotationSlider.Ratio = LogoHook.LogoRotation; // = 0
        };
        logoSection.Append(resetRotation);

        // --- Upload Logo ---
        OnOffTextButton fileChoose = new("Choose File", NoOnOff: true) { HAlign = 0.0f, Left = { Pixels = 10 }, Top = { Pixels = 94+25 } };
        fileChoose.OnLeftClick += (_, _) => LogoFileHelper.UploadFile();
        logoSection.Append(fileChoose);

        UIText fileText = new("No file chosen", 0.9f) { HAlign = 0.5f, Left = { Pixels = 50 }, Top = { Pixels = 94+25 } };
        logoSection.Append(fileText);

        // --- Update ---
        logoSection.OnUpdate += _ =>
        {
            logoScale.SetText($"Logo Scale: {LogoHook.LogoScale:F2}");
            logoRotation.SetText($"Logo Rotation: {LogoHook.LogoRotation:F2}");
        };

        list.Add(logoSection);
    }
    private void BuildDrawSection()
    {
        BoxSection drawSection = new(130 + 25);
        list.Add(drawSection);

        UIText drawHeader = new("Draw") { HAlign = 0.5f, Top = { Pixels = 4 } };
        drawSection.Append(drawHeader);

        OnOffTextButton logoToggle = new("Draw Logo: ") { Top = { Pixels = 30 } };
        logoToggle.OnLeftClick += (_, _) => LogoHook.IsDrawing = !LogoHook.IsDrawing;
        drawSection.Append(logoToggle);

        OnOffTextButton socialMediaToggle = new("Draw Social Buttons: ") { Top = { Pixels = 50 } };
        socialMediaToggle.OnLeftClick += (_, _) => SkipSocialMediaButtonsHook.IsDrawing = !SkipSocialMediaButtonsHook.IsDrawing;
        drawSection.Append(socialMediaToggle);

        OnOffTextButton versionNumberToggle = new("Draw Version Number: ") { Top = { Pixels = 70 } };
        versionNumberToggle.OnLeftClick += (_, _) => SkipVersionNumberDrawHook.IsDrawing = !SkipVersionNumberDrawHook.IsDrawing;
        drawSection.Append(versionNumberToggle);

        OnOffTextButton cloudsToggle = new("Draw Clouds: ") { Top = { Pixels = 90 } };
        cloudsToggle.OnLeftClick += (_, _) =>
        {
            //Main.cloudAlpha = 0;
            //Main.cloudBG = null;
            //Main.cloudBGActive = 0;
            //Main.cloud = null;
            SkipCloudsHook.IsDrawing = !SkipCloudsHook.IsDrawing;
        };
        drawSection.Append(cloudsToggle);

        OnOffTextButton backgroundToggle = new("Draw Background: ") { Top = { Pixels = 110 } };
        backgroundToggle.OnLeftClick += (_, _) => SkipBackgroundDrawHook.IsDrawing = !SkipBackgroundDrawHook.IsDrawing;
        drawSection.Append(backgroundToggle);
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
