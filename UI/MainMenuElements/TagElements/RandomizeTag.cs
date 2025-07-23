using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using UICustomizer.Common.Systems.Hooks.MainMenu;
using static UICustomizer.Common.Systems.MainMenu.MainMenuState;

namespace UICustomizer.UI.MainMenuElements.TagElements
{
    public class RandomizeTag : UIColoredImageButton
    {
        public RandomizeTag(Asset<Texture2D> texture, TextColorType type, bool isSmall = true) : base(texture, isSmall)
        {
            VAlign = 1f;
            Left.Set(80, 0);

            OnLeftMouseDown += (_, __) =>
            {
                var updatedColor = new Color(Main.rand.Next(256), Main.rand.Next(256), Main.rand.Next(256));

                var a = type switch
                {
                    TextColorType.Fill => MainMenuTextColorHook.NormalColor = updatedColor,
                    TextColorType.Outline => MainMenuOutlineTextColorHook.Color = updatedColor,
                    TextColorType.Hover => MainMenuTextColorHook.HoverColor = updatedColor,
                    _ => throw new System.NotImplementedException(),
                };
            };
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch sb)
        {
            base.Draw(sb);

            if (IsMouseHovering)
            {
                UICommon.TooltipMouseText("Randomize color");
            }
        }
    }
}