using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using UICustomizer.Common.Systems.Hooks.MainMenu;
using static UICustomizer.Common.Systems.MainMenu.MainMenuState;

namespace UICustomizer.UI.MainMenuElements.TagElements
{
    public class ResetTag : UIColoredImageButton
    {
        public ResetTag(Asset<Texture2D> texture, TextColorType type, bool isSmall = true) : base(texture, isSmall)
        {
            VAlign = 1f;
            Left.Set(0, 0);

            OnLeftMouseDown += (_, __) =>
            {
                Color c = type switch
                {
                    TextColorType.Fill => MainMenuFillTextColorHook.Color = Color.Gray,
                    TextColorType.Outline => MainMenuOutlineTextColorHook.Color = Color.Black,
                    TextColorType.Hover => MainMenuHoverTextColorHook.Color = Main.OurFavoriteColor,
                    _ => MainMenuFillTextColorHook.Color
                };
            };
        }
        public override void Update(GameTime gameTime)
        {
            HAlign = 1f;
            Top.Set(3f, 0f);
            Left.Set(6f, 0f);
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);
            //Log.Info(Main.MouseScreen.ToScreenPosition().ToString());

            if (IsMouseHovering)
            {
                DrawHelper.DrawTextAtMouse(sb, "Reset");
            }
        }
    }
}