using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;

namespace UICustomizer.MainMenu.UI
{
    public class ResetButton : UIColoredImageButton
    {
        public ResetButton() : base(Ass.Reset, true)
        {
            HAlign = 1f;
            Top.Set(3f, 0f);
            Left.Set(6f, 0f);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            //Log.Info(Main.MouseScreen.ToScreenPosition().ToString());

            if (IsMouseHovering)
            {
                UICommon.TooltipMouseText("Reset");
            }
        }
    }
}