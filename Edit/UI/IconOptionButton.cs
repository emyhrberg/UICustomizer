using System;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace UICustomizer.Edit.UI
{
    internal class IconOptionButton : UIColoredImageButton
    {
        private string name;

        public IconOptionButton(Asset<Texture2D> texture, string name, int index) : base(texture, isSmall: false)
        {
            this.name = name;
            Width.Set(40, 0);
            Height.Set(40, 0);
            HAlign = 0.5f;

            Top.Set(46 * index+6,0);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _backPanelTexture = Ass.DarkPanel;
            base.Draw(spriteBatch);

            if (IsMouseHovering)
            {
                Main.hoverItemName = $"{name}";
            }
        }
    }
}
