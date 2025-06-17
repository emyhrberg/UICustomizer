using System;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using UICustomizer.Common.Systems;
using UICustomizer.Helpers;

namespace UICustomizer.UI
{
    public class CollapsibleSection : UIElement
    {
        private float headerHeight = 30f;
        private readonly UIPanel headerPanel;
        private readonly CollapseIcon icon;
        private readonly UIText label;
        private readonly float contentHeight;
        private readonly Action<UIList> contentBuilder;
        private readonly Action onToggleAction;
        private readonly Action<UIElement> headerBuilder;

        public bool IsExpanded { get; private set; }

        public CollapsibleSection(
            string title,
            Action<UIList> buildContent,
            bool initialState = false,
            Action onToggle = null,
            Func<float> contentHeightFunc = null,
            Action<UIElement> buildHeader = null
        )
        {
            IsExpanded = initialState;
            contentHeight = contentHeightFunc?.Invoke() ?? 200f;
            contentBuilder = buildContent;
            onToggleAction = onToggle;
            headerBuilder = buildHeader;

            Width.Set(0, 1f);
            Height.Set(headerHeight + (IsExpanded ? contentHeight : 0), 0);

            // build header panel
            headerPanel = new UIPanel
            {
                Width = { Percent = 1f, Pixels = 0 },
                Left = { Percent = 0f, Pixels = 0 },
                Height = { Pixels = headerHeight }
            };
            headerPanel.SetPadding(0);
            Append(headerPanel);

            icon = new CollapseIcon(initialState)
            {
                VAlign = 0.5f
            };
            headerPanel.Append(icon);

            label = new UIText(title, 0.45f, large: true)
            {
                Left = { Pixels = 41 },
                VAlign = 0.5f
            };
            headerPanel.Append(label);

            headerBuilder?.Invoke(headerPanel);

            headerPanel.OnLeftClick += (evt, _) =>
            {
                UIElement target = evt.Target;
                while (target != null)
                {
                    if (target is CheckboxEyeElement || target is Button || target is ToggleAllEyeElement)
                        return;
                    target = target.Parent;
                }
                Toggle();
            };

            if (IsExpanded)
                AppendContent();
        }

        private void Toggle()
        {
            IsExpanded = !IsExpanded;
            onToggleAction?.Invoke();

            // remove old content
            foreach (var child in Children.ToArray())
                if (child != headerPanel)
                    RemoveChild(child);

            icon.SetExpanded(IsExpanded);
            Height.Set(headerHeight + (IsExpanded ? contentHeight : 0), 0);

            if (IsExpanded)
                AppendContent();

            Recalculate();
        }

        private void AppendContent()
        {
            var panel = new UIPanel
            {
                Top = { Pixels = headerHeight },
                Width = { Percent = 1f, Pixels = 0 },
                Left = { Percent = 0f, Pixels = 0f },
                Height = { Pixels = contentHeight }
            };
            panel.SetPadding(8);

            var list = new UIList
            {
                Width = { Percent = 1f, Pixels = 0 },
                Height = { Pixels = contentHeight }
            };
            panel.Append(list);

            contentBuilder(list);
            Append(panel);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            base.Draw(spriteBatch);

            // white underline beneath the header
            var dims = headerPanel.GetDimensions();
            int y = (int)(dims.Y + dims.Height);
            int w = (int)dims.Width - 14;
            //spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)dims.X+6, y-5, w, 1), Color.White);
        }
    }

    public class CollapseIcon : UIImage
    {
        private bool isExpanded;
        private bool isHovered;

        private bool _isHovering;
        private float _hoverTimer;
        private static readonly float HoverFadeTime = 0.2f;
        private float opacity;

        public CollapseIcon(bool initialExpanded = false)
          : base(initialExpanded ? Ass.Minus.Value : Ass.Plus.Value)
        {
            isExpanded = initialExpanded;
            ScaleToFit = true;
            AllowResizingDimensions = false;
            Width.Set(32, 0);
            Height.Set(32, 0);
            Left.Set(8, 0);
        }

        public void SetExpanded(bool expanded)
        {
            isExpanded = expanded;
            SetImage(isExpanded ? Ass.Minus.Value : Ass.Plus.Value);
            Width.Set(32, 0);
            Height.Set(32, 0);
            //Left.Set(4, 0);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            isHovered = true;
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
            isHovered = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (isHovered && IsMouseHovering)
                Main.hoverItemName = isExpanded ? "Collapse" : "Expand";
        }
    }
}
