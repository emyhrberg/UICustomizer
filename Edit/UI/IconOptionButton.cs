using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

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

            Top.Set(46 * index + 6, 0);
            SetVisibility(1f, 1f); // no dim change on hover
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
        }
        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _backPanelTexture = Ass.DarkPanel;
            _backPanelHighlightTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/CategoryPanelHighlight");

            base.Draw(spriteBatch);

            if (IsMouseHovering)
            {
                Main.hoverItemName = $"{name}";
            }
        }
    }
}
