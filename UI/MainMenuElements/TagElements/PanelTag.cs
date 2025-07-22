using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace UICustomizer.UI.MainMenuElements.TagElements
{
    internal class PanelTag : UIPanel
    {
        public UIText tagText;
        public PanelTag()
        {
            Height.Set(26, 0);
            HAlign = 1f;
            VAlign = 1f;
            Left.Set(-4, 0);
            Width.Set(95, 0);

            tagText = new UIText("#FFFFFF") { HAlign = .5f, VAlign = .5f };
            Append(tagText);
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
                Color c = HexToColor(tagText.Text);
                DrawHelper.DrawTextAtMouse(spriteBatch, $"R: {c.R}  G: {c.G}");
            }
        }

        public Color HexToColor(string hex)
        {
            if (hex.StartsWith("#"))
                hex = hex[1..]; // remove '#'

            if (hex.Length != 6)
                throw new ArgumentException("Hex must be 6 characters long.");

            byte r = Convert.ToByte(hex.Substring(0, 2), 16);
            byte g = Convert.ToByte(hex.Substring(2, 2), 16);
            byte b = Convert.ToByte(hex.Substring(4, 2), 16);

            return new Color(r, g, b);
        }

    }
}
