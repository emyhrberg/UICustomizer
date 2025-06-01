using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using static UICustomizer.UI.CollapsibleHeader;

namespace UICustomizer.UI
{
    public class CollapsibleHeader : UIElement
    {
        public enum CollapseState
        {
            Collapsed,
            Expanded
        }

        private string hoverText;
        public CollapseState collapseState;
        private CollapseIcon collapseIcon;
        private UIText textElement;

        public CollapsibleHeader(string text, CollapseState initialState = CollapseState.Expanded, Action onClick = null, Func<string> hoverText = null, float textScale = 0.5f, bool large = true)
        {
            Width.Set(0, 1f); // Full width
            Height.Set(24, 0);


            this.hoverText = hoverText?.Invoke() ?? "";

            // Icon on the left
            collapseIcon = new CollapseIcon(onClick)
            {
                Left = { Pixels = 5 },
                VAlign = 0.5f
            };
            Append(collapseIcon);

            // Text next to icon
            textElement = new UIText(text, textScale, large)
            {
                Left = { Pixels = 35 },
                VAlign = 0.5f
            };
            Append(textElement);

            // Set the initial state passed in
            SetCollapseState(initialState);
        }

        public void SetCollapseState(CollapseState state)
        {
            collapseState = state;
            collapseIcon?.UpdateState(state);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (IsMouseHovering && hoverText != "")
            {
                //Main.hoverItemName = hoverText;
                //UICommon.TooltipMouseText(tooltip);
            }
        }
    }

    public class CollapseIcon : UIImage
    {
        private CollapseState currentState = CollapseState.Expanded;

        public CollapseIcon(Action onClick) : base(Ass.Minuss.Value)
        {
            AllowResizingDimensions = false;
            ScaleToFit = true;
            Width.Set(23, 0);
            Height.Set(23, 0);

            OnLeftClick += (_, _) =>
            {
                // Toggle the state first
                if (Parent is CollapsibleHeader header)
                    header.SetCollapseState(header.collapseState == CollapseState.Expanded ? CollapseState.Collapsed : CollapseState.Expanded);
                // Then call the external onClick
                onClick?.Invoke();
            };

        }

        public void UpdateState(CollapseState state)
        {
            currentState = state;
            SetImage(state == CollapseState.Collapsed ? Ass.Pluss.Value : Ass.Minuss.Value);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            //Width.Set(23, 0);
            //Height.Set(23, 0);

            if (IsMouseHovering)
            {
                string tooltip = currentState == CollapseState.Expanded ? "Collapse" : "Expand";
                Main.hoverItemName = tooltip;
                //UICommon.TooltipMouseText(tooltip);
            }
        }
    }
}