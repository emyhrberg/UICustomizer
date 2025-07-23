using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using UICustomizer.Common.Systems.Hooks.MainMenu;
using static UICustomizer.Common.Systems.MainMenu.MainMenuState;

namespace UICustomizer.UI.MainMenuElements.TagElements
{
    public class ResetButton : UIColoredImageButton
    {
        public ResetButton(Asset<Texture2D> texture, TextColorType type, bool isSmall = true) : base(texture, isSmall)
        {
            HAlign = 1f;
            Top.Set(3f, 0f);
            Left.Set(6f, 0f);

            OnLeftMouseDown += (_, __) =>
            {
                Color c = type switch
                {
                    TextColorType.Fill => MainMenuTextColorHook.NormalColor = Color.Gray,
                    TextColorType.Outline => MainMenuOutlineTextColorHook.Color = Color.Black,
                    TextColorType.Hover => MainMenuTextColorHook.HoverColor = Main.OurFavoriteColor,
                    _ => MainMenuTextColorHook.NormalColor
                };
            };
        }
        public ResetButton(Asset<Texture2D> texture, bool isSmall = true) : base(texture, isSmall)
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
                //DrawHelper.DrawTextAtMouse(sb, "Reset");
                UICommon.TooltipMouseText("Reset");
            }
        }
    }
}