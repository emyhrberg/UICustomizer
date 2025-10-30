using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace UICustomizer.Core.MainMenuEditor.UI
{
    public class ConfigButton : UIImage
    {
        public ConfigButton(Asset<Texture2D> texture) : base(texture)
        {
            ImageScale = 0.62f;
            Top.Set(-3, 0);
            HAlign = 1f;
            Left.Set(-30, 0);
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
                //DrawHelper.DrawTextAtMouse(spriteBatch, "Config");
                UICommon.TooltipMouseText("Config");
            }
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (Conf.C == null) return;

            Conf.C.Open();
        }
    }
}