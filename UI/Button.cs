using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using UICustomizer.Common.Systems;

namespace UICustomizer.UI
{
    public class Button : UIPanel
    {
        public UIText buttonText;
        private readonly Func<string> tooltip;

        public Button(string text, Func<string> tooltip, Action onClick, int topOffset = 0, bool maxWidth = false, Action onRightClick = null, int width = 100)
        {
            // Variables
            this.tooltip = tooltip;
            OnMouseOver += (_, _) => BorderColor = Color.Yellow;
            OnMouseOut += (_, _) => BorderColor = Color.Black;
            OnRightClick += (_, _) => onRightClick?.Invoke();
            OnLeftClick += (_, _) =>
            {
                if (!UICustomizerSystem.EditModeActive)
                    return; // Ignore clicks if not in edit mode
                onClick?.Invoke();
            };

            // Panel size and position
            if (maxWidth)
                Width.Set(-16, 1f); // Full width
            else
                Width.Set(width, 0); // Fixed width
            Height.Set(30, 0);
            VAlign = 0.0f;
            HAlign = 0.0f;
            Top.Set(topOffset, 0);

            // Add UIText in the middle
            buttonText = new(text, 0.4f, true);
            buttonText.HAlign = 0.5f;
            buttonText.VAlign = 0.5f;
            Append(buttonText);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (IsMouseHovering)
            {
                UICommon.TooltipMouseText(tooltip());
            }
        }
    }
}