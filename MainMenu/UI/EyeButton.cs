using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace UICustomizer.UI.MainMenuElements
{
    public class EyeButton : UIImage
    {
        private Asset<Texture2D> onTex;
        private Asset<Texture2D> offTex;
        public bool isOn = true;

        public EyeButton(Asset<Texture2D> texture) : base(texture)
        {
            onTex = Ass.Inventory_Tick_On;
            offTex = Ass.Inventory_Tick_Off;

            ImageScale = 0.62f;

            HAlign = 1f;
            Left.Set(-60, 0);
            Top.Set(-3, 0);
        }

        public void Toggle()
        {
            isOn = !isOn;
            if (isOn)
            {
                SetImage(onTex);
            }
            else
            {
                SetImage(offTex);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Left.Set(-30-22-8, 0);

            base.Draw(spriteBatch);

            if (IsMouseHovering)
            {
                string showOrHide = isOn ? "Click to hide" : "Click to show";
                //DrawHelper.DrawTextAtMouse(spriteBatch, showOrHide);
                UICommon.TooltipMouseText(showOrHide);
                // Main.hoverItemName = "Config";
            }
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            Toggle();
        }
    }
}