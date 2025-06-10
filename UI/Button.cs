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

        private bool _isHovering;
        private float _hoverTimer;
        private static readonly float HoverFadeTime = 0.2f;
        private readonly Color _baseBg;
        private readonly Color _hoverBg;

        public Button(string text, Action onClick, Func<string> tooltip=default,  Action onRightClick = null, bool maxWidth=false, int width = 100, int height=30)
        {
            _baseBg = UICommon.DefaultUIBlue;
            _hoverBg = UICommon.DefaultUIBlueMouseOver * 0.1f;
            BackgroundColor = _baseBg;
            BorderColor = Color.Black;

            OnMouseOver += (_, _) => _isHovering = true;
            OnMouseOut += (_, _) => _isHovering = false;
            OnLeftClick += (_, _) =>
            {
                if (EditorSystem.IsActive)
                    onClick?.Invoke();
            };

            this.tooltip = tooltip;

            OnRightClick += (_, _) => onRightClick?.Invoke();
            // OnLeftMouseDown += (_, _) => BorderColor = Color.Yellow;
            // OnLeftMouseUp += (_, _) => BorderColor = Color.Black;
            // OnRightMouseDown += (_, _) => BorderColor = Color.Yellow;
            // OnRightMouseUp += (_, _) => BorderColor = Color.Black;
            OnLeftClick += (_, _) =>
            {
                if (EditorSystem.IsActive)
                    onClick?.Invoke();
            };

            // Panel size and position
            if (maxWidth)
                Width.Set(width, 1); // Fixed width
            else
                Width.Set(width, 0); // Fixed width
            Height.Set(height, 0);
            VAlign = 0.0f;
            HAlign = 0.0f;
            Top.Set(0, 0);
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
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // advance or reverse the hover timer
            if (_isHovering)
                _hoverTimer = Math.Min(0.2f, _hoverTimer + dt);
            else
                _hoverTimer = Math.Max(0f, _hoverTimer - dt);

            // only interpolate the border color
            float t = _hoverTimer / 0.2f;
            BorderColor = Color.Lerp(Color.Black, Color.Yellow, t);
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