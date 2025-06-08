using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using UICustomizer.Common.Systems;
using UICustomizer.Helpers;

namespace UICustomizer.UI
{
    /// <summary>
    /// A simple button UI element that can be clicked to perform an action.
    /// </summary>
    public class Button : UIPanel
    {
        public UIText buttonText;
        private readonly Func<string> tooltip;

        public Button(string text, Func<string> tooltip, Action onClick, int topOffset = 0, bool maxWidth = false, Action onRightClick = null, int width = 100)
        {
            BackgroundColor = UICommon.DefaultUIBlue;

            // Variables
            this.tooltip = tooltip;
            OnMouseOver += (_, _) =>
            {
                BackgroundColor = UICommon.DefaultUIBlueMouseOver * 0.1f;
                //BorderColor = Color.Yellow;
            };
            OnMouseOut += (_, _) =>
            {
                BackgroundColor = UICommon.DefaultUIBlue;
                BorderColor = Color.Black;
            };
            OnRightClick += (_, _) => onRightClick?.Invoke();
            OnLeftMouseDown += (_, _) =>
            {
                BorderColor = Color.Yellow;
            };
            OnLeftMouseUp += (_, _) =>
            {
                BorderColor = Color.Black;
            };
            OnRightMouseDown += (_, _) =>
            {
                BorderColor = Color.Yellow;
            };
            OnRightMouseUp += (_, _) =>
            {
                BorderColor = Color.Black;
            };
            OnLeftClick += (_, _) =>
            {
                if (!EditorSystem.IsActive)
                    return; // Ignore clicks if not in edit mode
                onClick?.Invoke();
            };

            // Panel size and position
            if (maxWidth)
            {
                Width.Set(-16, 1f); // Full width
                Left.Set(8, 0);
            }
            else
                Width.Set(width, 0); // Fixed width
            Height.Set(30, 0);
            VAlign = 0.0f;
            HAlign = 0.0f;
            Top.Set(topOffset, 0);
            Left.Set(0, 0);

            // Add UIText in the middle
            buttonText = new(text, 0.4f, true);
            buttonText.HAlign = 0.5f;
            buttonText.VAlign = 0.5f;
            Append(buttonText);
        }

        public void UpdateButtonText(string text)
        {
            buttonText.SetText(text, .4f, true);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (IsMouseHovering && !string.IsNullOrEmpty(tooltip()))
            {
                UICommon.TooltipMouseText(tooltip());
            }
        }
    }
}