using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;

namespace UICustomizer.UI.MainMenuElements
{
    internal class SmallColoredImageButton : UIColoredImageButton
    {
        public string tooltip;
        public SmallColoredImageButton(Asset<Texture2D> texture, string tooltip) : base(texture, true)
        {
            _color = Color.White;
            _texture = texture;

            _backPanelTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/SmallPanel");
            _backPanelHighlightTexture = Ass.SmallPanelHighlight;
            _backPanelBorderTexture = Main.Assets.Request<Texture2D>("Images/UI/CharCreation/SmallPanelBorder");

            Width.Set(_backPanelTexture.Width(), 0f);
            Height.Set(_backPanelTexture.Height(), 0f);

            this.tooltip = tooltip;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (IsMouseHovering)
            {
                UICommon.TooltipMouseText(tooltip);
            }
        }

    }
}
