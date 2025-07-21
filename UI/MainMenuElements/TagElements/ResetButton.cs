using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.OS;
using Terraria;
using Terraria.GameContent.UI.Elements;
using UICustomizer.Common.Systems.Hooks.MainMenu;

namespace UICustomizer.UI.MainMenuElements.TagElements
{
    public class ResetTag : UIColoredImageButton
    {
        public ResetTag(Asset<Texture2D> texture, bool isSmall = true) : base(texture, isSmall)
        {
            VAlign = 1f;
            Left.Set(0, 0);

            OnLeftMouseDown += (_, __) =>
            {
                MainMenuTextColorHook.MainMenuTextColor = Color.Black;
            };
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            //Log.Info(Main.MouseScreen.ToScreenPosition().ToString());
        }
    }
}