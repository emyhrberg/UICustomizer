using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using UICustomizer.Core.IngameEditor.System;
using UICustomizer.Core.IngameEditor.UI.Subpanels;

namespace UICustomizer.Core.IngameEditor.UI;

public sealed class EditPanel : UIPanel
{
    private enum Subpanel { None, Positions, Layouts, Settings }

    private readonly IconOptionButton _btnPositions;
    private readonly IconOptionButton _btnLayouts;
    private readonly IconOptionButton _btnSettings;

    private readonly PositionsPanel _positions;
    private readonly LayoutsPanel _layouts;
    private readonly SettingsPanel _settings;

    private Subpanel _active;

    private const int IconSize = 40;

    public EditPanel()
    {
        Width.Set(50, 0);
        Top.Set(600, 0);
        Left.Set(20, 0);
        Height.Set(6 + IconSize + 6 + IconSize + 6 + IconSize + 6, 0);
        SetPadding(0);

        BorderColor = new Color(89, 116, 213) * 0.9f;
        BackgroundColor = new Color(73, 94, 171) * 0.9f;

        _btnPositions = new IconOptionButton(Ass.P, "Positions", 0);
        _btnLayouts = new IconOptionButton(Ass.O, "Layouts", 1);
        _btnSettings = new IconOptionButton(Ass.S, "Settings", 2);

        _btnPositions.OnLeftClick += (_, _) => Toggle(Subpanel.Positions);
        _btnLayouts.OnLeftClick += (_, _) => Toggle(Subpanel.Layouts);
        _btnSettings.OnLeftClick += (_, _) => Toggle(Subpanel.Settings);

        Append(_btnPositions);
        Append(_btnLayouts);
        Append(_btnSettings);

        _positions = new PositionsPanel();
        _layouts = new LayoutsPanel();
        _settings = new SettingsPanel();

        _active = Subpanel.None;
        CloseActive();
    }

    private void CloseActive()
    {
        _positions.Remove();
        _layouts.Remove();
        _settings.Remove();

        _btnPositions.SetSelected(false);
        _btnLayouts.SetSelected(false);
        _btnSettings.SetSelected(false);

        _active = Subpanel.None;
    }

    private void Toggle(Subpanel target)
    {
        if (_active == target) { CloseActive(); return; }

        _positions.Remove();
        _layouts.Remove();
        _settings.Remove();
        _btnPositions.SetSelected(false);
        _btnLayouts.SetSelected(false);
        _btnSettings.SetSelected(false);

        UIPanel panelToShow = null;
        Vector2 pos = new(90, 450);
        Vector2 size = new(0, 0);

        switch (target)
        {
            case Subpanel.Positions:
                panelToShow = _positions;
                _btnPositions.SetSelected(true);
                size = new(250, 400);
                break;
            case Subpanel.Layouts:
                panelToShow = _layouts;
                _btnLayouts.SetSelected(true);
                size = new(250, 200);
                break;
            case Subpanel.Settings:
                panelToShow = _settings;
                _btnSettings.SetSelected(true);
                size = new(250, 200);
                break;
        }

        if (panelToShow != null)
        {
            var sys = ModContent.GetInstance<EditSystem>();
            sys.editState.Append(panelToShow);

            panelToShow.Left.Set(pos.X, 0f);
            panelToShow.Top.Set(pos.Y, 0f);
            panelToShow.Width.Set(size.X, 0f);
            panelToShow.Height.Set(size.Y, 0f);

            Remove();
            sys.editState.Append(this);

            _active = target;
        }
    }

    public override void LeftClick(UIMouseEvent evt)
    {
        var sys = ModContent.GetInstance<EditSystem>();
        if (!sys.Enabled)
            return;

        base.LeftClick(evt);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var sys = ModContent.GetInstance<EditSystem>();
        if (!sys.Enabled)
            return;

        Top.Set(600, 0);

        base.Draw(spriteBatch);
    }
}