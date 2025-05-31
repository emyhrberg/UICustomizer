using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;

namespace UICustomizer.UI
{
    public class Button : UIPanel
    {
        public UIText buttonText;
        public Action click;
        private readonly string hoverText;

        public Button(string text, string hoverText, int topOffset, Action click)
        {
            // Variables
            this.click = click;
            this.hoverText = hoverText;

            // Panel size and position
            Width.Set(70, 0);
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

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);

            BorderColor = Color.Yellow;
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);

            BorderColor = Color.Black;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            click?.Invoke();
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
                UICommon.TooltipMouseText(hoverText);
            }
        }
    }
}