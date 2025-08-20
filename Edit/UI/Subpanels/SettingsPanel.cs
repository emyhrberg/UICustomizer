using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using UICustomizer.Edit.Helpers;
using UICustomizer.Edit.System;

namespace UICustomizer.Edit.UI.Subpanels;

internal sealed class SettingsPanel : UIPanel
{
    private readonly List<Row> _rows = new();

    private sealed class Row : UIElement
    {
        private readonly Func<bool> _get;
        private readonly Action<bool> _set;
        private readonly UIText _label;
        private readonly UIPanel _toggle;
        private bool _last;

        public Row(string label, Func<bool> getter, Action<bool> setter)
        {
            _get = getter;
            _set = setter;

            Width.Set(0, 1f);
            Height.Set(24, 0);

            _label = new UIText(label, 0.9f) { VAlign = 0.5f, Left = { Pixels = 6 } };
            Append(_label);

            _toggle = new UIPanel
            {
                Width = { Pixels = 44 },
                Height = { Pixels = 18 },
                VAlign = 0.5f,
                HAlign = 1f,
                Left = { Pixels = -6 }
            };
            _toggle.SetPadding(0);
            _toggle.OnLeftClick += (_, _) => _set(!_get());

            Append(_toggle);

            UpdateVisual();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (_last != _get())
                UpdateVisual();
        }

        private void UpdateVisual()
        {
            bool v = _get();
            _last = v;
            _toggle.BackgroundColor = v
                ? new Color(50, 145, 65) * 0.85f   // ON
                : new Color(145, 50, 50) * 0.75f;  // OFF
            _toggle.BorderColor = v ? new Color(80, 200, 90) : new Color(180, 70, 70);

            _toggle.RemoveAllChildren();
            var txt = new UIText(v ? "ON" : "OFF", 0.85f)
            { HAlign = 0.5f, VAlign = 0.5f, TextColor = Color.White };
            _toggle.Append(txt);
        }
    }

    public SettingsPanel()
    {
        BorderColor = new Color(89, 116, 213) * 0.9f;
        BackgroundColor = new Color(73, 94, 171) * 0.9f;

        Width.Set(320, 0);
        Height.Set(180, 0);
        Left.Set(90, 0);
        Top.Set(0, 0);

        SetPadding(8);

        Build();
    }

    private void Build()
    {
        // rows stack
        float y = 6;
        void AddRow(string label, Func<bool> getter, Action<bool> setter)
        {
            var row = new Row(label, getter, setter);
            row.Top.Set(y, 0);
            Append(row);
            _rows.Add(row);
            y += 26;
        }

        // Wire up to your live flags. "Edit Mode" toggles EditSystem.Enabled.
        var sys = ModContent.GetInstance<EditSystem>();

        AddRow("Edit Mode",
            getter: () => sys.Enabled,
            setter: v =>
            {
                if (v != sys.Enabled)
                    sys.Toggle(); // keep single source of truth
            });

        AddRow("Show hitboxes", () => EditorFlags.ShowHitboxes, v => EditorFlags.ShowHitboxes = v);
        AddRow("Show element names", () => EditorFlags.ShowNames, v => EditorFlags.ShowNames = v);
        AddRow("Fit hitbox bounds", () => EditorFlags.FitBounds, v => EditorFlags.FitBounds = v);
        AddRow("Snap to edges", () => EditorFlags.SnapToEdges, v => EditorFlags.SnapToEdges = v);
        AddRow("Show layer toggle", () => EditorFlags.ShowLayerToggle, v => EditorFlags.ShowLayerToggle = v);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        BackgroundColor = ColorHelper.DarkBluePanel * 0.75f;
        BorderColor = Color.Black * 0.45f;

        base.Draw(spriteBatch);
    }

    public override void LeftClick(UIMouseEvent evt)
    {
        base.LeftClick(evt);
    }
}