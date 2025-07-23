using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;

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
                Color c = ColorHelper.HexToColor(tagText.Text);
                UICommon.TooltipMouseText($"R: {c.R}  G: {c.G} B; {c.B}");
            }
        }
    }
}
