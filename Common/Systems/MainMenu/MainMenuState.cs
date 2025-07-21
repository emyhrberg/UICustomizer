using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using UICustomizer.Common.Configs;
using UICustomizer.Common.Systems.Hooks.MainMenu;
using UICustomizer.UI.MainMenuElements;
using UICustomizer.UI.MainMenuElements.TagElements;

namespace UICustomizer.Common.Systems.MainMenu;

internal sealed class MainMenuState : UIState
{
    // Parent Elements
    private UIText headerText;
    private ExpandablePanel panel;        // collapsible panel (timeHeader + body)
    private UIList list;       // inner list that lives inside the panel

    // Text Color Section
    private BoxSection textColorSection;
    private ResetTag resetTag;
    private HueSlider hueSlider;   // color picker for text color
    private CopyTag copy;
    private PasteTag paste;
    private RandomizeTag rand;
    private PanelTag panelTag;

    // Text Color Values
    private Color _lastColor = MainMenuTextColorHook.MainMenuTextColor;

    // Time Section
    private BoxSection timeSection;
    private TimeSlider timeSlider;

    
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
            MinHeight = { Pixels = 330 }, // enough to show both sections
            //Height = { Pixels = Main.screenHeight - 100 },
            Top = { Pixels = 30f },
            ListPadding = 4f
        };
        panel.VisibleWhenExpanded.Add(list);

        BuildTextColorSection();
        BuildTimeSection();
    }

    private void BuildTextColorSection()
    {
        textColorSection = new();

        // Text color header and reset icon is a top element
        UIElement topRow = new UIElement
        {
            Width = { Percent = 1f },
            Height = { Pixels = 24f }
        };

        UIText textColorHeader = new("Text Color")
        {
            Top = { Pixels = 4f },
            VAlign = 0.5f
        };

        resetTag = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Copy"));
        resetTag.Top.Set(0f, 0f);
        resetTag.Left.Pixels = textColorHeader.MinWidth.Pixels + 50f; // adjust as needed
        resetTag.HAlign = 0f;

        topRow.Append(textColorHeader);
        topRow.Append(resetTag);

        textColorSection.Append(topRow);

        hueSlider = new();
        textColorSection.Append(hueSlider);

        copy = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Copy"));
        textColorSection.Append(copy);

        paste = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Paste"));
        textColorSection.Append(paste);

        rand = new(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Randomize"));
        textColorSection.Append(rand);

        panelTag = new();
        textColorSection.Append(panelTag);

        list.Add(textColorSection);
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

        UIText pauseToggle = new("Pause: Off") { Top = { Pixels = 74 }, HAlign = 0.5f };
        timeSection.Append(pauseToggle);

        void refresh()
        {
            timeText.SetText(WorldTimeHelper.GetFormattedTime());
            timeSlider.Ratio = WorldTimeHelper.GetRatioFromTime();
            pauseToggle.SetText($"Pause: {(MainMenuPauseSystem.TimeIsPausedBySlider ? "On" : "Off")}");
        }

        pauseToggle.OnLeftMouseDown += (_, __) =>
        {
            MainMenuPauseSystem.TimeIsPausedBySlider = !MainMenuPauseSystem.TimeIsPausedBySlider;
            refresh();
        };

        timeSection.OnUpdate += _ => refresh();
    }

    public override void Draw(SpriteBatch sb)
    {
        base.Draw(sb);

        DrawHoverTooltips(sb);
    }

    private void DrawHoverTooltips(SpriteBatch sb)
    {
        UIElement hovered = null;

        if (copy.IsMouseHovering) hovered = copy;
        else if (paste.IsMouseHovering) hovered = paste;
        else if (rand.IsMouseHovering) hovered = rand;

        string text = hovered switch
        {
            CopyTag => "Copy color",
            PasteTag => "Paste color",
            RandomizeTag => "Randomize color",
            _ => null
        };

        if (text != null)
        {
            DrawHelper.DrawTextAtMouse(sb, text);
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // Sync UI widgets if someone changed the colour behind their back
        Color current = MainMenuTextColorHook.MainMenuTextColor;

        if (current != _lastColor)
        {
            _lastColor = current;

            // 1) move the hue slider knob  (HSV hue in [0,1])
            float h = Main.rgbToHsl(current).X;
            hueSlider.Ratio = h;

            // 2) update the little #RRGGBB label
            panelTag.tagText.SetText($"#{current.R:X2}{current.G:X2}{current.B:X2}");
        }
    }
}
