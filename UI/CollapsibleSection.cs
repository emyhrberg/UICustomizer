using System;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;
using UICustomizer.Helpers;

namespace UICustomizer.UI
{
    public class CollapsibleSection : UIElement
    {
        private float _headerHeight = 30f;
        private readonly UIPanel _header;
        private readonly CollapseIcon _icon; // Changed to CollapseIcon
        private readonly UIText _label;
        private readonly float _contentHeight;
        private readonly Action<UIElement> _buildContent;
        private readonly Action _onToggle;
        private readonly Action<UIElement> _buildHeader;

        public bool IsExpanded { get; private set; }

        public CollapsibleSection(
          string title,
          bool initialState,
          Action<UIElement> buildContent,
          Action onToggle,
          Func<float> contentHeight = null,
          Action<UIElement> buildHeader = null
        )
        {
            IsExpanded = initialState;
            _contentHeight = contentHeight?.Invoke() ?? 200f;
            _buildContent = buildContent;
            _onToggle = onToggle;
            _buildHeader = buildHeader;
            Width.Set(0, 1f);
            Height.Set(_headerHeight + (IsExpanded ? _contentHeight : 0), 0);

            // header bar
            _header = new UIPanel
            {
                Width = { Percent = 1f, Pixels = -4f },
                Left = { Percent = 0f, Pixels = 2f },
                Height = { Pixels = _headerHeight }
            };
            _header.SetPadding(0);
            Append(_header);

            _icon = new CollapseIcon(initialState)
            {
                Left = { Pixels = 8 },
                VAlign = 0.5f
            };
            _header.Append(_icon);

            _label = new UIText(title, 0.4f, true)
            {
                Left = { Pixels = 34 }, // Adjusted for 26px icon + padding
                VAlign = 0.5f
            };
            _header.Append(_label);

            buildHeader?.Invoke(_header);

            _header.OnLeftClick += (evt, _) =>
            {
                // We dont want to toggle when clicking on a checkbox
                UIElement elm = evt.Target;
                while (elm != null)
                {
                    // if _any_ parent in the hierarchy is a Checkbox, skip toggling
                    if (elm is Checkbox)
                        return;
                    elm = elm.Parent;
                }

                Toggle();
            };

            // if we're expanded initially, build & append the content
            if (IsExpanded)
                AppendContent();
        }

        private void Toggle()
        {
            // flip state + notify caller
            IsExpanded = !IsExpanded;
            _onToggle?.Invoke();

            // remove old content (if any)
            foreach (var c in Children.ToArray())
                if (c != _header)
                    RemoveChild(c);

            // Update icon state
            _icon.SetExpanded(IsExpanded);
            Height.Set(_headerHeight + (IsExpanded ? _contentHeight : 0), 0);

            // rebuild content if now expanded
            if (IsExpanded)
                AppendContent();

            Recalculate();
        }

        private void AppendContent()
        {
            var content = new UIPanel
            {
                //BackgroundColor = Color.DarkGray,
                //BorderColor = Color.DarkGray,
                Top = { Pixels = _headerHeight },
                Width = { Percent = 1f, Pixels = -4f },
                Left = { Percent = 0f, Pixels = 2f },
                Height = { Pixels = _contentHeight }
            };
            content.SetPadding(8);
            _buildContent(content);
            Append(content);
        }
    }

    public class CollapseIcon : UIImage
    {
        private bool _isExpanded;
        private bool _isHovered;

        public CollapseIcon(bool initialExpanded = false) : base(initialExpanded ? Ass.Minus.Value : Ass.Plus.Value)
        {
            _isExpanded = initialExpanded;
            ScaleToFit = true;
            AllowResizingDimensions = false; // Don't allow resizing to maintain
            Width.Set(22, 0); // Force size
            Height.Set(22, 0);
            Left.Set(5, 0);
        }

        public void SetExpanded(bool expanded)
        {
            _isExpanded = expanded;
            SetImage(_isExpanded ? Ass.Minus.Value : Ass.Plus.Value);
            Width.Set(22, 0);
            Height.Set(22, 0);
            Left.Set(5, 0);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            _isHovered = true;
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
            _isHovered = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // Show tooltip when hovered
            if (_isHovered && IsMouseHovering)
            {
                string tooltipText = _isExpanded ? "Collapse" : "Expand";
                Main.hoverItemName = tooltipText;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}